using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Ambev.DeveloperEvaluation.Common.Events;

/// <summary>
/// Redis Pub/Sub implementation of <see cref="IEventPublisher"/>.
/// Publishes serialized domain events to Redis channels.
/// </summary>
public class RedisEventPublisher : IEventPublisher
{
    private readonly ILogger<RedisEventPublisher> _logger;
    private readonly Lazy<IConnectionMultiplexer?> _lazyMultiplexer;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new JsonStringEnumConverter() }
    };

    public RedisEventPublisher(
        ILogger<RedisEventPublisher> logger,
        IConfiguration configuration,
        IConnectionMultiplexer? redis = null)
    {
        _logger = logger;
        _lazyMultiplexer = new Lazy<IConnectionMultiplexer?>(() =>
        {
            if (redis != null)
                return redis;

            var connectionString = configuration.GetConnectionString("Redis")
                ?? configuration["Redis:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            try
            {
                return ConnectionMultiplexer.Connect(connectionString);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Could not establish connection to Redis at '{RedisConnection}'. Redis publishing deactivated for this instance.",
                    connectionString);
                return null;
            }
        });
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(string channel, TEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var eventName = typeof(TEvent).Name;
        var redis = _lazyMultiplexer.Value;

        if (redis == null || !redis.IsConnected)
        {
            _logger.LogWarning(
                "Redis is not connected or not configured. Domain event {EventType} to channel '{Channel}' was not dispatched to Redis.",
                eventName,
                channel);
            return;
        }

        try
        {
            var payload = JsonSerializer.Serialize(@event, JsonOptions);
            var subscriber = redis.GetSubscriber();

            await subscriber.PublishAsync(RedisChannel.Literal(channel), payload);

            _logger.LogInformation(
                "Domain event {EventType} published successfully to Redis channel '{Channel}'.",
                eventName,
                channel);

            _logger.LogDebug(
                "Payload for domain event {EventType} on channel '{Channel}': {Payload}",
                eventName,
                channel,
                payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish domain event {EventType} to Redis channel '{Channel}'.",
                eventName,
                channel);
        }
    }

    /// <inheritdoc />
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
    {
        var channel = ResolveDefaultChannelName(typeof(TEvent));
        return PublishAsync(channel, @event, cancellationToken);
    }

    /// <summary>
    /// Derives a standard channel name from the event type.
    /// E.g., SaleCreatedEvent -> sales.created
    /// </summary>
    private static string ResolveDefaultChannelName(Type type)
    {
        var name = type.Name;
        if (name.EndsWith("Event", StringComparison.OrdinalIgnoreCase))
            name = name[..^5];

        if (name.StartsWith("Sale", StringComparison.OrdinalIgnoreCase))
        {
            var suffix = name[4..];
            if (string.IsNullOrEmpty(suffix))
                return "sales.events";

            return $"sales.{suffix.ToLowerInvariant()}";
        }

        return $"events.{name.ToLowerInvariant()}";
    }
}

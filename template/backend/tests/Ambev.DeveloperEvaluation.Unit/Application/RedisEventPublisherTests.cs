using System.Text.Json;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using StackExchange.Redis;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class RedisEventPublisherTests
{
    private readonly ILogger<RedisEventPublisher> _logger = Substitute.For<ILogger<RedisEventPublisher>>();
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly IConnectionMultiplexer _redis = Substitute.For<IConnectionMultiplexer>();
    private readonly ISubscriber _subscriber = Substitute.For<ISubscriber>();

    public RedisEventPublisherTests()
    {
        _redis.GetSubscriber(Arg.Any<object>()).Returns(_subscriber);
    }

    [Fact(DisplayName = "GIVEN null event WHEN publishing THEN throws ArgumentNullException")]
    public async Task PublishAsync_NullEvent_ThrowsArgumentNullException()
    {
        var publisher = new RedisEventPublisher(_logger, _configuration, _redis);

        var act = () => publisher.PublishAsync<SaleCreatedEvent>("sales.created", null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "GIVEN null redis multiplexer and empty config WHEN publishing THEN does not throw and skips dispatch")]
    public async Task PublishAsync_NullMultiplexer_DoesNotThrow()
    {
        var publisher = new RedisEventPublisher(_logger, _configuration, null);
        var sale = SaleTestData.GenerateValidSale();
        var domainEvent = new SaleCreatedEvent(sale);

        var act = () => publisher.PublishAsync("sales.created", domainEvent);

        await act.Should().NotThrowAsync();
    }

    [Fact(DisplayName = "GIVEN disconnected redis multiplexer WHEN publishing THEN does not throw and skips dispatch")]
    public async Task PublishAsync_DisconnectedRedis_DoesNotThrow()
    {
        _redis.IsConnected.Returns(false);
        var publisher = new RedisEventPublisher(_logger, _configuration, _redis);
        var sale = SaleTestData.GenerateValidSale();
        var domainEvent = new SaleCreatedEvent(sale);

        var act = () => publisher.PublishAsync("sales.created", domainEvent);

        await act.Should().NotThrowAsync();
        await _subscriber.DidNotReceive().PublishAsync(Arg.Any<RedisChannel>(), Arg.Any<RedisValue>(), Arg.Any<CommandFlags>());
    }

    [Fact(DisplayName = "GIVEN connected redis WHEN publishing THEN serializes and dispatches to redis subscriber")]
    public async Task PublishAsync_ConnectedRedis_PublishesToSubscriber()
    {
        _redis.IsConnected.Returns(true);
        var publisher = new RedisEventPublisher(_logger, _configuration, _redis);
        var sale = SaleTestData.GenerateValidSale();
        var domainEvent = new SaleCreatedEvent(sale);

        await publisher.PublishAsync("sales.created", domainEvent);

        await _subscriber.Received(1).PublishAsync(
            Arg.Is<RedisChannel>(c => c == "sales.created"),
            Arg.Is<RedisValue>(v => ((string)v!).Contains(sale.SaleNumber)),
            Arg.Any<CommandFlags>());
    }

    [Fact(DisplayName = "GIVEN event without channel name WHEN publishing THEN resolves default channel and publishes")]
    public async Task PublishAsync_DefaultChannelOverload_PublishesToResolvedChannel()
    {
        _redis.IsConnected.Returns(true);
        var publisher = new RedisEventPublisher(_logger, _configuration, _redis);
        var sale = SaleTestData.GenerateValidSale();
        var domainEvent = new SaleCreatedEvent(sale);

        await publisher.PublishAsync(domainEvent);

        await _subscriber.Received(1).PublishAsync(
            Arg.Is<RedisChannel>(c => c == "sales.created"),
            Arg.Is<RedisValue>(v => ((string)v!).Contains(sale.SaleNumber)),
            Arg.Any<CommandFlags>());
    }

    [Fact(DisplayName = "GIVEN redis exception WHEN publishing THEN logs error and does not bubble exception")]
    public async Task PublishAsync_RedisThrowsException_CatchesAndLogsError()
    {
        _redis.IsConnected.Returns(true);
        _subscriber.PublishAsync(Arg.Any<RedisChannel>(), Arg.Any<RedisValue>(), Arg.Any<CommandFlags>())
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Connection lost"));

        var publisher = new RedisEventPublisher(_logger, _configuration, _redis);
        var sale = SaleTestData.GenerateValidSale();
        var domainEvent = new SaleCreatedEvent(sale);

        var act = () => publisher.PublishAsync("sales.created", domainEvent);

        await act.Should().NotThrowAsync();
    }
}

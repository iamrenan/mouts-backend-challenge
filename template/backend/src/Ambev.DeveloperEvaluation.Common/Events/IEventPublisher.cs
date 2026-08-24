namespace Ambev.DeveloperEvaluation.Common.Events;

/// <summary>
/// Defines a contract for publishing domain events to message brokers or event streams (e.g. Redis).
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to a specified channel.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="channel">The destination channel or topic.</param>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<TEvent>(string channel, TEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes an event using a default resolved channel name.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}

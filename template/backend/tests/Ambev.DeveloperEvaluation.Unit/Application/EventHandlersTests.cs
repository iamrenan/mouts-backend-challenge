using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class EventHandlersTests
{
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();

    [Fact(DisplayName = "GIVEN SaleCreatedEvent WHEN handled THEN writes log and publishes to Redis channel")]
    public async Task SaleCreatedEventHandler_Handle_PublishesEventToRedis()
    {
        // Given
        var logger = Substitute.For<ILogger<SaleCreatedEventHandler>>();
        var handler = new SaleCreatedEventHandler(logger, _eventPublisher);
        var sale = SaleTestData.GenerateValidSale();
        var domainEvent = new SaleCreatedEvent(sale);

        // When
        await handler.Handle(domainEvent, CancellationToken.None);

        // Then
        await _eventPublisher.Received(1).PublishAsync("sales.created", domainEvent, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GIVEN SaleModifiedEvent WHEN handled THEN writes log and publishes to Redis channel")]
    public async Task SaleModifiedEventHandler_Handle_PublishesEventToRedis()
    {
        // Given
        var logger = Substitute.For<ILogger<SaleModifiedEventHandler>>();
        var handler = new SaleModifiedEventHandler(logger, _eventPublisher);
        var sale = SaleTestData.GenerateValidSale();
        var domainEvent = new SaleModifiedEvent(sale);

        // When
        await handler.Handle(domainEvent, CancellationToken.None);

        // Then
        await _eventPublisher.Received(1).PublishAsync("sales.modified", domainEvent, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GIVEN SaleCancelledEvent WHEN handled THEN writes log and publishes to Redis channel")]
    public async Task SaleCancelledEventHandler_Handle_PublishesEventToRedis()
    {
        // Given
        var logger = Substitute.For<ILogger<SaleCancelledEventHandler>>();
        var handler = new SaleCancelledEventHandler(logger, _eventPublisher);
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();
        var domainEvent = new SaleCancelledEvent(sale);

        // When
        await handler.Handle(domainEvent, CancellationToken.None);

        // Then
        await _eventPublisher.Received(1).PublishAsync("sales.cancelled", domainEvent, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GIVEN SaleItemCancelledEvent WHEN handled THEN writes log and publishes to Redis channel")]
    public async Task SaleItemCancelledEventHandler_Handle_PublishesEventToRedis()
    {
        // Given
        var logger = Substitute.For<ILogger<SaleItemCancelledEventHandler>>();
        var handler = new SaleItemCancelledEventHandler(logger, _eventPublisher);
        var saleItem = new SaleItem(Guid.NewGuid(), "Test Product", 2, 50m);
        var domainEvent = new SaleItemCancelledEvent(saleItem);

        // When
        await handler.Handle(domainEvent, CancellationToken.None);

        // Then
        await _eventPublisher.Received(1).PublishAsync("sales.item_cancelled", domainEvent, Arg.Any<CancellationToken>());
    }
}

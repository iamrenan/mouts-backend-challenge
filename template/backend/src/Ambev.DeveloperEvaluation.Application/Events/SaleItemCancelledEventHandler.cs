using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Events;

public class SaleItemCancelledEventHandler(
    ILogger<SaleItemCancelledEventHandler> logger,
    IEventPublisher eventPublisher) : INotificationHandler<SaleItemCancelledEvent>
{
    public async Task Handle(SaleItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "SaleItem {ItemId} cancelled. Product: {ProductName} ({ProductId}), Quantity: {Quantity}, UnitPrice: {UnitPrice:C}, Total: {Total:C}",
            notification.SaleItem.Id,
            notification.SaleItem.ProductName,
            notification.SaleItem.ProductId,
            notification.SaleItem.Quantity,
            notification.SaleItem.UnitPrice,
            notification.SaleItem.Total);

        await eventPublisher.PublishAsync("sales.item_cancelled", notification, cancellationToken);
    }
}

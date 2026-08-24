using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Events;

public class SaleModifiedEventHandler(
    ILogger<SaleModifiedEventHandler> logger,
    IEventPublisher eventPublisher) : INotificationHandler<SaleModifiedEvent>
{
    public async Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Sale {SaleId} updated. SaleNumber: {SaleNumber}, TotalAmount: {TotalAmount:C}, ItemsCount: {ItemsCount}",
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.TotalAmount,
            notification.Sale.Items.Count);

        await eventPublisher.PublishAsync("sales.modified", notification, cancellationToken);
    }
}

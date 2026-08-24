using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Events;

public class SaleCreatedEventHandler(
    ILogger<SaleCreatedEventHandler> logger,
    IEventPublisher eventPublisher) : INotificationHandler<SaleCreatedEvent>
{
    public async Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Sale {SaleId} created. SaleNumber: {SaleNumber}, Customer: {CustomerName} ({CustomerId}), Branch: {BranchName} ({BranchId}), TotalAmount: {TotalAmount:C}, ItemsCount: {ItemsCount}",
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.CustomerName,
            notification.Sale.CustomerId,
            notification.Sale.BranchName,
            notification.Sale.BranchId,
            notification.Sale.TotalAmount,
            notification.Sale.Items.Count);

        await eventPublisher.PublishAsync("sales.created", notification, cancellationToken);
    }
}

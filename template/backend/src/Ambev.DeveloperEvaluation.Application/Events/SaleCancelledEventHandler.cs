using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Events;

public class SaleCancelledEventHandler(
    ILogger<SaleCancelledEventHandler> logger,
    IEventPublisher eventPublisher) : INotificationHandler<SaleCancelledEvent>
{
    public async Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Sale {SaleId} cancelled. SaleNumber: {SaleNumber}, Status: Cancelled, TotalAmount: {TotalAmount:C}",
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.TotalAmount);

        await eventPublisher.PublishAsync("sales.cancelled", notification, cancellationToken);
    }
}

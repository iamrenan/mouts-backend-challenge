using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public record SaleItemCancelledEvent(SaleItem SaleItem);
}
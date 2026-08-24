using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleItemCancelledEvent(SaleItem SaleItem) : INotification;

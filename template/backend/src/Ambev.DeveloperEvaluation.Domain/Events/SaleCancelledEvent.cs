using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleCancelledEvent(Sale Sale) : INotification;
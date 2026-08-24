
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleList;

public record GetSaleListQuery(int Page, int PageSize) : IRequest<GetSaleListResult>;
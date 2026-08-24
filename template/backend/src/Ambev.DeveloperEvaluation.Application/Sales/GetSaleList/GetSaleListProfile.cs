using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleList;

/// <summary>
/// Profile for mapping between Sale entity and GetSaleListResponse
/// </summary>
public class GetSaleListProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSaleList operation
    /// </summary>
    public GetSaleListProfile()
    {
        CreateMap<GetSaleListQuery, Sale>();
        CreateMap<Sale, GetSaleListResult>();
    }
}

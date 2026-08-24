using Ambev.DeveloperEvaluation.Application.Sales.GetSaleList;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleList;

public class GetSaleListProfile : Profile
{
    public GetSaleListProfile()
    {
        CreateMap<GetSaleListRequest, GetSaleListQuery>();
    }
}

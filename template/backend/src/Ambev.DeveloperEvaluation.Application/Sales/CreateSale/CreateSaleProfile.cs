using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Profile for mapping between Sale entity and CreateSaleResult
/// </summary>
public class CreateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateSale operation
    /// </summary>
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .AfterMap((cmd, sale) =>
            {
                sale.SetCustomer(cmd.CustomerId, cmd.CustomerName);
                sale.SetBranch(cmd.BranchId, cmd.BranchName);
                if (cmd.Items != null)
                {
                    foreach (var item in cmd.Items)
                    {
                        sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
                    }
                }
            });

        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, CreateSaleItemResult>();
    }
}

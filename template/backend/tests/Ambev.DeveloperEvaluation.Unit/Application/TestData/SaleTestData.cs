using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class SaleTestData
{
    private static readonly Faker<CreateSaleItemCommand> CreateItemFaker = new Faker<CreateSaleItemCommand>()
        .CustomInstantiator(f => new(
            Guid.NewGuid(),
            f.Commerce.ProductName(),
            f.Random.Int(1, 20),
            f.Random.Decimal(10, 500)
        ));

    private static readonly Faker<CreateSaleCommand> CreateSaleFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchId, f => Guid.NewGuid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => CreateItemFaker.Generate(f.Random.Int(1, 5)));

    public static CreateSaleCommand GenerateValidCreateCommand() => CreateSaleFaker.Generate();

    public static CreateSaleCommand GenerateInvalidCreateCommand() => new();

    public static Sale GenerateValidSale()
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid()
        };
        sale.SetCustomer(Guid.NewGuid(), "Test Customer");
        sale.SetBranch(Guid.NewGuid(), "Test Branch");
        sale.AddItem(Guid.NewGuid(), "Product A", 5, 50.0m);
        sale.Initialize();
        return sale;
    }

    public static UpdateSaleCommand GenerateValidUpdateCommand(Guid id)
    {
        return new UpdateSaleCommand(
            id,
            Guid.NewGuid(),
            "Updated Customer",
            Guid.NewGuid(),
            "Updated Branch",
            [new(Guid.NewGuid(), "Updated Product", 2, 100m)]
        );
    }
}
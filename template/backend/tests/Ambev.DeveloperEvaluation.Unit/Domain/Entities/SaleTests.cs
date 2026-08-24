using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Initialize sets SaleNumber and SaleDate")]
    public void Given_NewSale_When_Initialized_Then_SetsSaleNumberAndDate()
    {
        // Arrange
        var sale = new Sale();

        // Act
        sale.Initialize();

        // Assert
        sale.SaleNumber.Should().StartWith("SALE-");
        sale.SaleDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory(DisplayName = "AddItem calculates discount correctly based on quantity")]
    [InlineData(3, 100, 0, 300)]      // < 4 items: no discount
    [InlineData(4, 100, 40, 360)]     // 4 items: 10% discount (400 - 40 = 360)
    [InlineData(9, 100, 90, 810)]     // 9 items: 10% discount (900 - 90 = 810)
    [InlineData(10, 100, 200, 800)]   // 10 items: 20% discount (1000 - 200 = 800)
    [InlineData(20, 100, 400, 1600)]  // 20 items: 20% discount (2000 - 400 = 1600)
    public void Given_Sale_When_AddingItems_Then_CalculatesDiscountCorrectly(
        int quantity, decimal unitPrice, decimal expectedDiscount, decimal expectedTotal)
    {
        // Arrange
        var sale = new Sale();
        var productId = Guid.NewGuid();

        // Act
        sale.AddItem(productId, "Product", quantity, unitPrice);

        // Assert
        var item = sale.Items.First();
        item.Discount.Should().Be(expectedDiscount);
        item.Total.Should().Be(expectedTotal);
        sale.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "AddItem throws DomainException when quantity exceeds 20")]
    public void Given_Sale_When_AddingMoreThan20Items_Then_ThrowsDomainException()
    {
        // Arrange
        var sale = new Sale();

        // Act
        var act = () => sale.AddItem(Guid.NewGuid(), "Product", 21, 10);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot sell more than 20*");
    }

    [Fact(DisplayName = "CancelItem marks item as cancelled and recalculates total")]
    public void Given_SaleWithItems_When_ItemCancelled_Then_RecalculatesTotal()
    {
        // Arrange
        var sale = new Sale();
        var product1 = Guid.NewGuid();
        var product2 = Guid.NewGuid();
        sale.AddItem(product1, "P1", 2, 50); // Total 100
        sale.AddItem(product2, "P2", 2, 50); // Total 100
        sale.TotalAmount.Should().Be(200);

        // Act
        sale.CancelItem(product1);

        // Assert
        sale.Items.First(i => i.ProductId == product1).IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(100);
    }

    [Fact(DisplayName = "Adding same product in multiple items calculates discount based on total quantity across entire sale")]
    public void Given_Sale_When_AddingSameProductInMultipleItems_Then_CalculatesDiscountBasedOnTotalQuantityAcrossSale()
    {
        // Arrange
        var sale = new Sale();
        var productId = Guid.NewGuid();

        // Act - 3 items in group 1, 3 items in group 2 -> total 6 items (10% tier)
        sale.AddItem(productId, "Pasta", 3, 10);
        sale.AddItem(productId, "Pasta", 3, 10);

        // Assert
        sale.Items.Should().HaveCount(2);
        sale.Items.ElementAt(0).Discount.Should().Be(3.00m); // 3 * 10 * 0.10
        sale.Items.ElementAt(0).Total.Should().Be(27.00m);
        sale.Items.ElementAt(1).Discount.Should().Be(3.00m); // 3 * 10 * 0.10
        sale.Items.ElementAt(1).Total.Should().Be(27.00m);
        sale.TotalAmount.Should().Be(54.00m);
    }

    [Fact(DisplayName = "Adding same product spread out exceeding 20 total quantity throws DomainException")]
    public void Given_Sale_When_AddingSameProductExceeding20InTotal_Then_ThrowsDomainException()
    {
        // Arrange
        var sale = new Sale();
        var productId = Guid.NewGuid();

        // Act
        sale.AddItem(productId, "Pasta", 11, 10);
        var act = () => sale.AddItem(productId, "Pasta", 10, 10);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot sell more than 20*");
    }

    [Fact(DisplayName = "UpdateItems with empty list cancels all items and sets TotalAmount to 0")]
    public void Given_SaleWithItems_When_SyncItemsWithEmptyList_Then_CancelsAllItemsAndSetsTotalToZero()
    {
        // Arrange
        var sale = new Sale();
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        sale.AddItem(p1, "P1", 2, 50);
        sale.AddItem(p2, "P2", 2, 50);
        sale.TotalAmount.Should().Be(200);

        // Act
        sale.UpdateItems(Enumerable.Empty<(Guid, string, int, decimal)>());

        // Assert
        sale.TotalAmount.Should().Be(0);
        sale.Items.Should().OnlyContain(i => i.IsCancelled && i.Total == 0 && i.Discount == 0);
    }

    [Fact(DisplayName = "Cancel marks sale and all items as cancelled with zeroed totals")]
    public void Given_SaleWithItems_When_Cancelled_Then_CancelsAllItems()
    {
        // Arrange
        var sale = new Sale();
        sale.AddItem(Guid.NewGuid(), "P1", 2, 50);
        sale.AddItem(Guid.NewGuid(), "P2", 2, 50);

        // Act
        sale.Cancel();

        // Assert
        sale.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(0);
        sale.Items.Should().OnlyContain(i => i.IsCancelled && i.Total == 0 && i.Discount == 0);
    }

    [Fact(DisplayName = "AddItem throws InvalidOperationException when sale is cancelled")]
    public void Given_CancelledSale_When_AddingItem_Then_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = new Sale();
        sale.Cancel();

        // Act
        var act = () => sale.AddItem(Guid.NewGuid(), "P1", 1, 10);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot add items to a cancelled sale*");
    }
}
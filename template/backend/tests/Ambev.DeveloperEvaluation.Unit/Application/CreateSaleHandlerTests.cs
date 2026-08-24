using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "GIVEN valid sale data WHEN creating sale THEN returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = SaleTestData.GenerateValidCreateCommand();
        var sale = SaleTestData.GenerateValidSale();
        var result = new CreateSaleResult(
            sale.Id,
            sale.SaleNumber,
            sale.SaleDate,
            sale.CustomerId,
            sale.CustomerName,
            sale.BranchId,
            sale.BranchName,
            [],
            sale.TotalAmount
        );

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(sale.Id);
        createSaleResult.SaleNumber.Should().NotBeNullOrEmpty();
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Is<SaleCreatedEvent>(e => e.Sale == sale), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GIVEN invalid sale data WHEN creating sale THEN throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = SaleTestData.GenerateInvalidCreateCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "GIVEN items with same ProductId but different ProductNames WHEN creating sale THEN throws validation exception")]
    public async Task Handle_SameProductIdDifferentNames_ThrowsValidationException()
    {
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Items = [
                new(productId, "Product 1", 2, 10m),
                new(productId, "Product 2", 3, 10m)
            ]
        };

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "GIVEN items with same ProductId but different UnitPrices WHEN creating sale THEN throws validation exception")]
    public async Task Handle_SameProductIdDifferentPrices_ThrowsValidationException()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Items = [
                new(productId, "Product 1", 2, 10m),
                new(productId, "Product 1", 3, 15m)
            ]
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
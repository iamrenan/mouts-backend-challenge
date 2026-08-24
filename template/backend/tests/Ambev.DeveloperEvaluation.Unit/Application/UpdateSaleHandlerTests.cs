using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
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

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "GIVEN valid update command WHEN updating sale THEN updates sale and returns result")]
    public async Task Handle_ValidRequest_UpdatesSaleSuccessfully()
    {
        var sale = SaleTestData.GenerateValidSale();
        var command = SaleTestData.GenerateValidUpdateCommand(sale.Id);
        var result = new UpdateSaleResult(
            sale.Id,
            sale.SaleNumber,
            sale.SaleDate,
            command.CustomerId!.Value,
            command.CustomerName!,
            command.BranchId!.Value,
            command.BranchName!,
            [],
            sale.TotalAmount,
            false
        );

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(result);

        var updateResult = await _handler.Handle(command, CancellationToken.None);

        updateResult.Should().NotBeNull();
        updateResult.Id.Should().Be(sale.Id);
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Is<SaleModifiedEvent>(e => e.Sale == sale), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GIVEN update command for non-existing sale WHEN updating sale THEN throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        var command = SaleTestData.GenerateValidUpdateCommand(Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GIVEN update command with null items WHEN updating sale THEN succeeds")]
    public async Task Handle_NullItems_UpdatesSaleSuccessfully()
    {
        var sale = SaleTestData.GenerateValidSale();
        var command = new UpdateSaleCommand(
            sale.Id,
            null,
            "New Customer Name",
            null,
            null,
            null
        );

        var result = new UpdateSaleResult(
            sale.Id,
            sale.SaleNumber,
            sale.SaleDate,
            sale.CustomerId,
            "New Customer Name",
            sale.BranchId,
            sale.BranchName,
            [],
            sale.TotalAmount,
            false
        );

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(result);

        var updateResult = await _handler.Handle(command, CancellationToken.None);

        updateResult.Should().NotBeNull();
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GIVEN update command with items having same ProductId but different names WHEN updating THEN throws validation exception")]
    public async Task Handle_SameProductIdDifferentNames_ThrowsValidationException()
    {
        var productId = Guid.NewGuid();
        var command = new UpdateSaleCommand(
            Guid.NewGuid(),
            null,
            null,
            null,
            null,
            [
                new(productId, "Product 1", 2, 10m),
                new(productId, "Product 2", 3, 10m)
            ]
        );

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "GIVEN update command with items having same ProductId but different prices WHEN updating THEN throws validation exception")]
    public async Task Handle_SameProductIdDifferentPrices_ThrowsValidationException()
    {
        var productId = Guid.NewGuid();
        var command = new UpdateSaleCommand(
            Guid.NewGuid(),
            null,
            null,
            null,
            null,
            [
                new(productId, "Product 1", 2, 10m),
                new(productId, "Product 1", 3, 15m)
            ]
        );

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "GIVEN update command with empty items list WHEN updating sale THEN cancels existing items")]
    public async Task Handle_EmptyItemsList_CancelsExistingItems()
    {
        var sale = SaleTestData.GenerateValidSale();
        var command = new UpdateSaleCommand(
            sale.Id,
            null,
            null,
            null,
            null,
            []
        );

        var result = new UpdateSaleResult(
            sale.Id,
            sale.SaleNumber,
            sale.SaleDate,
            sale.CustomerId,
            sale.CustomerName,
            sale.BranchId,
            sale.BranchName,
            new List<UpdateSaleItemResult>(),
            0m,
            false
        );

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(result);

        var updateResult = await _handler.Handle(command, CancellationToken.None);

        updateResult.Should().NotBeNull();
        sale.TotalAmount.Should().Be(0m);
        sale.Items.Should().OnlyContain(i => i.IsCancelled);
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }
}
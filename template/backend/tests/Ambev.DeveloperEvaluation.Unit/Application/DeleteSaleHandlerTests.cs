using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new DeleteSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "GIVEN existing sale ID WHEN deleting sale THEN deletes sale and returns success")]
    public async Task Handle_ExistingSale_ReturnsSuccess()
    {
        var sale = SaleTestData.GenerateValidSale();
        var command = new DeleteSaleCommand(sale.Id);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.DeleteAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        await _saleRepository.Received(1).DeleteAsync(sale.Id, Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Is<SaleCancelledEvent>(e => e.Sale == sale), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GIVEN non-existing sale ID WHEN deleting sale THEN throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GIVEN missing ID WHEN deleting sale THEN throws validation exception")]
    public async Task Handle_MissingId_ThrowsValidationException()
    {
        var command = new DeleteSaleCommand(Guid.Empty);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
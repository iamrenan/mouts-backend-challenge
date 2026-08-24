using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "GIVEN existing sale ID WHEN getting sale THEN returns sale details")]
    public async Task Handle_ExistingSale_ReturnsSaleDetails()
    {
        var saleId = Guid.NewGuid();
        var query = new GetSaleQuery { Id = saleId };
        var sale = new Sale();
        var result = new GetSaleResult(
            saleId,
            "SALE-123",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [],
            100m
        );

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        var getSaleResult = await _handler.Handle(query, CancellationToken.None);

        getSaleResult.Should().NotBeNull();
        getSaleResult.Id.Should().Be(saleId);
        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GIVEN non-existing sale ID WHEN getting sale THEN throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        var query = new GetSaleQuery { Id = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var act = () => _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GIVEN empty ID WHEN getting sale THEN throws validation exception")]
    public async Task Handle_EmptyId_ThrowsValidationException()
    {
        var query = new GetSaleQuery { Id = Guid.Empty };

        var act = () => _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
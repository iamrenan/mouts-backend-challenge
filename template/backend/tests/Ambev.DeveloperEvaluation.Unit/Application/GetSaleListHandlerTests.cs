using Ambev.DeveloperEvaluation.Application.Sales.GetSaleList;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleListHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleListHandler _handler;

    public GetSaleListHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleListHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid query When getting sale list Then returns paginated results")]
    public async Task Handle_ValidQuery_ReturnsPaginatedResults()
    {
        // Given
        var query = new GetSaleListQuery(1, 10);
        var sales = new List<Sale> { SaleTestData.GenerateValidSale(), SaleTestData.GenerateValidSale() };

        _saleRepository.GetAllAsync(1, 10, Arg.Any<CancellationToken>()).Returns(sales);
        _saleRepository.CountAsync(Arg.Any<CancellationToken>()).Returns(2);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Sales.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact(DisplayName = "Given invalid query with negative page When getting sale list Then throws validation exception")]
    public async Task Handle_InvalidPage_ThrowsValidationException()
    {
        // Given
        var query = new GetSaleListQuery(0, 10);

        // When
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
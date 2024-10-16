using Hermes.Builders;
using Hermes.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HermesIntegrationTests.Repositories;

public class UnitUnderTestRepositoryTests
{
    private readonly UnitUnderTestRepository _sut;
    private readonly HermesLocalContext _localContext;
    private readonly UnitUnderTestBuilder _unitUnderTestBuilder;

    public UnitUnderTestRepositoryTests(UnitUnderTestBuilder unitUnderTestBuilder, HermesLocalContext hermesLocalContext)
    {
        this._unitUnderTestBuilder = unitUnderTestBuilder;
        this._localContext = hermesLocalContext;
        this._sut = new UnitUnderTestRepository(_localContext);
    }

    [Fact]
    public async Task AddAndSaveAsync_ValidUnitUnderTest_AddsAndSaves()
    {
        var uut = _unitUnderTestBuilder.Build();
        await _sut.AddAndSaveAsync(uut);
        var result = _sut.GetById(uut.Id);
        Assert.NotNull(result);
        Assert.Equal(uut.Id, result.Id);
    }

    [Fact]
    public async Task Delete_ValidUnitUnderTest_Deletes()
    {
        var uut = _unitUnderTestBuilder.Build();
        await _sut.AddAndSaveAsync(uut);
        _sut.Delete(uut.Id);
        await _sut.SaveChangesAsync();
        var result = _sut.GetById(uut.Id);
        Assert.Null(result);
    }

    [Fact]
    public void Edit_ValidUnitUnderTest_Edits()
    {
        var uut = _unitUnderTestBuilder.Build();
        _sut.Edit(uut);
        Assert.Equal(EntityState.Modified, _localContext.Entry(uut).State);
    }

    [Fact]
    public async Task GetAll_ValidUnitUnderTest_GetsAll()
    {
        const int listLength = 3;
        for (int i = 0; i < listLength; i++)
        {
            var uut = _unitUnderTestBuilder.Build();
            await _sut.AddAndSaveAsync(uut);
        }

        Assert.True(listLength <= _sut.GetAll().Count);
    }
}
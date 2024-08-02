using Hermes.Builders;
using Hermes.Common.Validators;
using Hermes.Models;
using Hermes.Repositories;
using Moq;

namespace HermesTests.Common.Validators;

public class AnyDefectsWithin1HourValidatorTests
{
    private readonly SfcResponseBuilder _sfcResponseBuilder;

    public AnyDefectsWithin1HourValidatorTests(SfcResponseBuilder sfcResponseBuilder)
    {
        this._sfcResponseBuilder = sfcResponseBuilder;
    }

    [Fact]
    public async void ValidateAsync_WithAnyDefects_ReturnStop()
    {
        var defect = new Defect();
        var defectRepositoryMock = GetDefectRepositoryMock(defect);
        var sfcResponse = _sfcResponseBuilder.Build();

        var sut = new AnyDefectsWithin1HourValidator(defectRepositoryMock);
        Assert.False((await sut.ValidateAsync(sfcResponse)).IsNull);
    }

    [Fact]
    public async void ValidateAsync_NotConsecutiveDefects_ReturnStopNull()
    {
        var defectRepositoryMock = GetDefectRepositoryMock(Defect.Null);
        var sfcResponse = _sfcResponseBuilder.Build();

        var sut = new AnyDefectsWithin1HourValidator(defectRepositoryMock);
        Assert.True((await sut.ValidateAsync(sfcResponse)).IsNull);
    }

    private IDefectRepository GetDefectRepositoryMock(Defect? defect = null)
    {
        var defectRepositoryMock = new Mock<IDefectRepository>();
        defectRepositoryMock
            .Setup(x => x.GetAnyDefectsWithin1Hour(It.IsAny<int>()))
            .Returns(Task.FromResult(defect ?? Defect.Null));
        return defectRepositoryMock.Object;
    }
}
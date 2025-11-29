using ApiRefactor.Domain;
using NUnit.Framework;
namespace ApiRefactor.Tests.Domain;

[TestFixture]
public class WaveEntityTests
{
    [Test]
    public void NewWave_HasGuid()
    {
        var wave = new Wave();
        Assert.That(wave.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Name_Defaults_ToEmpty()
    {
        var wave = new Wave();
        Assert.That(wave.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void WaveDate_CanBeSet()
    {
        var dt = new DateTime(2025, 11, 29, 18, 35, 59, DateTimeKind.Utc);
        var wave = new Wave { WaveDate = dt };
        Assert.That(wave.WaveDate, Is.EqualTo(dt));
    }
}
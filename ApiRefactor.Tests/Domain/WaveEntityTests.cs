using ApiRefactor.Domain;
using NUnit.Framework;
namespace ApiRefactor.Tests.Domain;

[TestFixture]
/// <summary>
/// Tests for the Wave entity.
/// </summary>
public class WaveEntityTests
{   /// <summary>       
    /// Tests that a new Wave has a non-empty GUID. 
    /// </summary>
    [Test]
    public void NewWave_HasGuid()
    {
        var wave = new Wave();
        Assert.That(wave.Id, Is.Not.EqualTo(Guid.Empty));
    }
    /// <summary>
    /// Verifies that a newly created Wave instance has its Name property set to an empty string by default.
    /// </summary>
    /// <remarks>Ensures Wave.Name is empty by default..</remarks>
    [Test]
    public void Name_Defaults_ToEmpty()
    {
        var wave = new Wave();
        Assert.That(wave.Name, Is.EqualTo(string.Empty));
    }
    /// <summary>
    /// Tests the WaveDate property of the Wave entity.
    /// </summary>
    [Test]
    public void WaveDate_CanBeSet()
    {
        var waveDate = new DateTime(2025, 11, 29, 18, 35, 59, DateTimeKind.Utc);
        var wave = new Wave { WaveDate = waveDate };
        Assert.That(wave.WaveDate, Is.EqualTo(waveDate));
    }
}
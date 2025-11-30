using System;

namespace ApiRefactor.Domain;
/// <summary>
/// Represents a wave event, including its unique identifier, name, and scheduled date.
/// </summary>
public class Wave
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime WaveDate { get; set; }

    // Generate a unique identifier for the wave
    public Wave()
    {
        Id = Guid.NewGuid();
    }
}


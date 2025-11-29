
using System;

namespace ApiRefactor.Domain;

public class Wave
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime WaveDate { get; set; }
}


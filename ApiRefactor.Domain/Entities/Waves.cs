using System;

namespace ApiRefactor.Domain;
/// <summary>
/// Represents a collection of wave objects.
/// </summary>
public class Waves
{
    public List<Wave> Items { get; set; } = new();
}
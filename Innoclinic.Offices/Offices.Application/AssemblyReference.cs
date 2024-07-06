using System.Reflection;

namespace Offices.Application;

/// <summary>
/// Provides a reference to the assembly for the application layer
/// and holds a reference to the current assembly
/// </summary>
public class AssemblyReference
{
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
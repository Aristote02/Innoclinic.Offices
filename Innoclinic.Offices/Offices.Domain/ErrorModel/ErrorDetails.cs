using System.Text.Json;

namespace Offices.Domain.ErrorModel;

/// <summary>
/// Represents details of an error
/// </summary>
public class ErrorDetails
{
	public int StatusCode { get; init; }
	public string? Message { get; init; }

	public override string ToString() => JsonSerializer.Serialize(this);
}

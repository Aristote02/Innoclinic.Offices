namespace Offices.Domain.Exceptions;

/// <summary>
/// Represents a class for exceptions for not found entities 
/// </summary>
public class NotFoundException : Exception
{
	/// <summary>
	/// Initializes a new instance of the NotFoundException class 
	/// with a specified error message
	/// </summary>
	/// <param name="message">The message that describes the error</param>
	public NotFoundException(string message) :
		base(message)
	{

	}
}
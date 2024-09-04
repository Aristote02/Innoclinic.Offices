using FluentValidation;
using Innoclinic.Shared.Requests.Offices;

namespace Offices.Application.Validators;

/// <summary>
/// Validator for OfficeRequest objects
/// </summary>
public class OfficeRequestValidator : AbstractValidator<OfficeRequest>
{
	/// <summary>
	/// Configures the validation rules
	/// </summary>
	public OfficeRequestValidator()
	{
		RuleFor(office => office.Address)
			.NotEmpty().WithMessage("The Address is a required field")
			.MaximumLength(100).WithMessage("The address cannot exceed 100 characters")
			.Matches("[A-Za-z0-9'\\.\\-\\s\\,]+$")
			.WithMessage("Invalid address format. Alphanumeric characters, spaces, commas, hyphens, and periods are allowed.");

		RuleFor(office => office.RegistryPhoneNumber)
			.NotEmpty().WithMessage("Registry phone number is required")
			.Matches(@"^\+?(\d{1,3}\s?)?(\(?\d{1,4}\)?[\s-]?)?(\d{1,4}[\s-]?)?(\d{1,4}[\s-]?)?(\d{1,9})$")
			.WithMessage("Invalid phone number format. follow this formats: " +
			"'+375 25-710-33-51', '+1 555-123-4567', '555-123-4567', '(555) 123-4567', '+44 20 1234 5678'");

		RuleFor(office => office.IsActive)
			.NotNull().WithMessage("IsActive must not be null")
			.WithErrorCode("NotNullValidator")
			.Must(value => value == true || value == false)
			.WithMessage("Invalid IsActive value. It must be either true or false.");
	}
}
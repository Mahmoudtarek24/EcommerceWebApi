
namespace Bussines_Logic.Validation
{
	public class AgeRangeAttribute :ValidationAttribute
	{
		private readonly int minAge = 14;
		private readonly int maxAge;
		public AgeRangeAttribute(int maxAge)
		{
			this.maxAge = maxAge;
			if (maxAge < minAge)
				ErrorMessage = "Maximum age cannot be less than minimum age.";
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if(value is DateTime dateOfBirth)
			{
				var age=DateTime.Now.Year-dateOfBirth.Year;
				if (age > maxAge||age<minAge)
				{
					return new ValidationResult($"Customer age must be between {minAge} and {maxAge} years.");
				}

			}
			return ValidationResult.Success;	
		}
	}
}

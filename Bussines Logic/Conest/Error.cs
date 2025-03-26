
namespace Bussines_Logic.Conest
{
	public static class Error
	{
		public const string RequiredFiled = "{0} is required.";
		public const string MinLength = "{0} must be at least {1} characters.";
		public const string MaxLength = "{0} cannot exceed {1} characters.";
		public const string EmailAddress = "Invalid Email Address.";
		public const string PhoneNumber = "Invalid Phone Number.";
		public const string MatchPassword = "New Password and Confirm New Password do not match.";
		public const string StringLent  = "{0} must be between {1} and {2} characters.";
		public const string NotAllowedExtension = "Only .png, .jpg, .jpeg files are allowed!";
		public const string MaxSize = "File cannot be more that 2 MB!";
		public const string RangValue = "{0} must be between {1}% and {2}%.";
		public const string Rang = "{0} must be between {1} and {2}.";
		public const string	stockQuntity = "The quantity for this product is currently zero, so its availability cannot be changed until stock is added. At the moment, this item is marked as unavailable. Please update the stock quantity to make it available again.";
		public const string ConfirmPasswordNotMatch = "The password and confirmation password do not match.";
	}
}

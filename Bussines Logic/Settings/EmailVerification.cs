
namespace Bussines_Logic.Settings
{
	public class EmailVerification
	{
		private Dictionary<string, string> emailVerification=new Dictionary<string, string>();
		
		public string GenerateCode(string email)
		{
			Random random= new Random();	
		   var code=random.Next(12344,99999).ToString();
			emailVerification[email] = code;
			return code;
		}

		public bool CheckCode(string email,string code)
		{
			if(emailVerification.TryGetValue(email,out string StordCode))
			{
				return code== StordCode;	
			}
		    return false;
		}

	}
}

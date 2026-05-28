using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Security
{
	public class LoginOTP : BaseEntity
	{
		public LoginOTP()
		{
			IsOTPExpired = false;
		} 
		public string UserID { get; set; }
		public string Password { get; set; }
		public string OTP { get; set; }
		public DateTime GenerateDate { get; set; }
		public DateTime OTPExpireDate { get; set; }
		public string MobileNo { get; set; } 
		public bool IsOTPExpired { get; set; }
	}
}

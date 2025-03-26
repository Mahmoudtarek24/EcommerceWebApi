using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussines_Logic.DTO.FeedBackDto
{
	public class FeedbackDeleteDTO
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int FeedbackId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }
	}
}

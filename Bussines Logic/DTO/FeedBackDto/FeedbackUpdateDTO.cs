using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussines_Logic.DTO.FeedBackDto
{
	public class FeedbackUpdateDTO
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int FeedbackId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		[Range(1, 10, ErrorMessage = Error.Rang)]
		public double Rating { get; set; }
		[StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
		public string Comment { get; set; }
	}
}

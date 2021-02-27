using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LongUrl.Models
{
	public class AccessToken
	{
		public int Id { get; set; }
		[Required][MinLength(5)][DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		public string Token { get; set; }
		[NotMapped]
		public string ResultMessage { get; set; }

		public void GenerateNew()
		{
			if (string.IsNullOrEmpty(Email)) throw new ArgumentNullException(nameof(Email));

			Token = Email.GetHashCode(StringComparison.OrdinalIgnoreCase).ToString();
		}
	}
}

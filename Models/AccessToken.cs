using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LongUrl.Models
{
	public class AccessToken
	{
		public int Id { get; set; }
		[Required][MinLength(5)][DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		public string Token { get; set; }
		public DateTime Timestamp { get; set; }
		[NotMapped]
		public string ResultMessage { get; set; }

		public void GenerateNew()
		{
			if (string.IsNullOrEmpty(Email)) throw new ArgumentNullException(nameof(Email));

			using (SHA1Managed sha1 = new SHA1Managed())
			{
				Token = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(Email))).Replace("-", "");
			}
			Timestamp = DateTime.Now;
		}
	}
}

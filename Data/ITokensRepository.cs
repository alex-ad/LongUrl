using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Models;

namespace LongUrl.Data
{
	public interface ITokensRepository
	{
		public IQueryable<AccessToken> AccessTokens { get; }

		public void AddToken(AccessToken token);
		public bool IsTokenValid(string token);
	}
}

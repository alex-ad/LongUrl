using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Models;
using Microsoft.EntityFrameworkCore;

namespace LongUrl.Data
{
	public class TokenRepository : ITokensRepository
	{
		private readonly TokenDbContext _context;

		public TokenRepository(TokenDbContext context)
		{
			_context = context;
		}

		public IQueryable<AccessToken> AccessTokens => _context.AccessTokens;

		public void AddToken(AccessToken token)
		{
			_context.AddAsync(token);
			_context.SaveChangesAsync();
		}

		public bool IsTokenValid(string token)
		{
			if (!_context.AccessTokens.Any()) return false;

			var t = _context.AccessTokens.FirstOrDefaultAsync(x =>
				x.Token.Equals(token));

			return t.Result != null;
		}
	}
}

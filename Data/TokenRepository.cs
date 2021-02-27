using System;
using System.Linq;
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
			if (_context.AccessTokens == null || !_context.AccessTokens.Any()) return false;

			var t = _context.AccessTokens.FirstOrDefaultAsync(x =>
				x.Token.Equals(token)).Result;

			if (t == null) return false;

			var dt = DateTime.Now;
			TimeSpan ts = dt - t.Timestamp;

			if (ts.TotalSeconds < 30) return false;

			t.Timestamp = dt;
			_context.Update(t);
			_context.SaveChangesAsync();

			return true;
		}
	}
}

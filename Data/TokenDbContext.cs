using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Models;
using Microsoft.EntityFrameworkCore;

namespace LongUrl.Data
{
	public class TokenDbContext : DbContext
	{
		public TokenDbContext(DbContextOptions<TokenDbContext> options) : base(options) { }

		public DbSet<AccessToken> AccessTokens { get; set; }
	}
}

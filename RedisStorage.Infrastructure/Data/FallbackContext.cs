using Microsoft.EntityFrameworkCore;
using RedisStorage.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisStorage.Infrastructure.Data
{
    public class FallbackContext : DbContext
    {
        public FallbackContext(DbContextOptions<FallbackContext> options) : base (options)
        { 
        }

        public DbSet<Meter> Meters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly OrderDbContext _db;

        public DbInitializer(OrderDbContext db)
        {
            _db = db;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                     OrderDbContextSeed.SeedAsync(_db);
                }
            }
            catch (Exception)
            {
            }

        }
    }
}

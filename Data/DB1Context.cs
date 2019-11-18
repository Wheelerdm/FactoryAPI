using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FactoryAPI;

namespace FactoryAPI.Models
{
    public class DB1Context : DbContext
    {
        public DB1Context (DbContextOptions<DB1Context> options)
            : base(options)
        {
        }

        public DbSet<FactoryAPI.Factory> Factory { get; set; }
    }
}

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingPong.Models;

namespace PingPong.Data
{
    public class PingPongContext : DbContext
    {
        public PingPongContext (DbContextOptions<PingPongContext> options)
            : base(options)
        {
        }

        public DbSet<PingPong.Models.Player> Player { get; set; }

        public DbSet<PingPong.Models.Team> Team { get; set; }

        public DbSet<PingPong.Models.Game> Game { get; set; }
    }
}

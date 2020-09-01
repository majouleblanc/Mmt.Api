using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.Models
{
    public class MmtContext : IdentityDbContext<MmtUser>
    {
        public MmtContext(DbContextOptions<MmtContext> options)
               : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<TourCuriosity>().HasKey(tc => new { tc.TourId, tc.CuriosityId });
            builder.Entity<TourCuriosity>().HasOne(tc => tc.Curiosity).WithMany(tc => tc.ToursCuriosities).HasForeignKey(tc => tc.CuriosityId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<TourCuriosity>().HasOne(tc => tc.Tour).WithMany(tc => tc.ToursCuriosities).HasForeignKey(tc => tc.TourId).OnDelete(DeleteBehavior.Restrict);
        }


        //public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Curiosity> Curiosities { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<CuriosityLike> CuriosityLikes { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourCuriosity> tourCuriosities { get; set; }
    }
}

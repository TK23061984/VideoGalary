using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Galary.Models
{
    public class GalaryContext: DbContext
    {
        public GalaryContext()
            : base("name=GalaryDb")
        {
            Database.SetInitializer<GalaryContext>(null);
        }

        public DbSet<Users> Users { get; set; }

        public DbSet<Roles> Roles { get; set; }

        public DbSet<UsersRoles> UsersRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>().HasMany(u => u.AssignedRoles).WithRequired(r => r.Users);
            modelBuilder.Entity<Roles>().HasMany(r => r.AllowedUsers).WithRequired(ur => ur.Roles);
            modelBuilder.Entity<Roles>().HasMany(r => r.AllowedVideoTypes).WithRequired(vr => vr.Roles);
            modelBuilder.Entity<VideoTypes>().HasMany(r => r.AssignedRoles).WithRequired(v => v.VideoTypes);
            /*modelBuilder.Entity<Users>().
                HasMany<Roles>(u => u.AssignedRoles).
                WithMany(r => r.AllowedUsers).
                Map(ur =>
                    {
                        ur.MapLeftKey("UserID");
                        ur.MapRightKey("RoleID");
                        ur.ToTable("UsersRoles");
                    });
             * */

        }

        public DbSet<VideoTypes> VideoTypes { get; set; }

        public DbSet<Videos> Videos { get; set; }

        public DbSet<VideoTypeRoles> VideoTypeRoles { get; set; }

        public System.Data.Entity.DbSet<Galary.Models.LoginViewModel> LoginViewModels { get; set; }

    }
}
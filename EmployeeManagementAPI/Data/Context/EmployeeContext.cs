using EmployeeManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementAPI.Data.Context
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options):base(options)
        {

        }
        public DbSet<UserModel> UserModels { get; set; }
        public DbSet<RoleModel> RoleModels { get; set; }
        public DbSet<LeaveModel> LeaveModels { get; set; }
        public DbSet<DepartmentModel> DepartmentModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().ToTable("tbl_User");
            modelBuilder.Entity<LeaveModel>().ToTable("tbl_Leave");
            modelBuilder.Entity<DepartmentModel>().ToTable("tbl_Department");
            modelBuilder.Entity<RoleModel>().ToTable("tbl_Role");
        }
    }
}

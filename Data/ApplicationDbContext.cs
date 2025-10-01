using LeaveManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LeaveManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }


        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
        public DbSet<LeaveAllocation> LeaveAllocations => Set<LeaveAllocation>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<LeaveAllocation>()
            .HasIndex(a => new { a.UserId, a.LeaveTypeId, a.Year })
            .IsUnique();


            builder.Entity<LeaveRequest>()
            .HasOne(r => r.RequestingUser)
            .WithMany()
            .HasForeignKey(r => r.RequestingUserId)
            .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<LeaveRequest>()
            .HasOne(r => r.ApprovedBy)
            .WithMany()
            .HasForeignKey(r => r.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
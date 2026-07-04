using Eportal.Modules.Exams.Domain;
using Microsoft.EntityFrameworkCore;

namespace Eportal.Modules.Exams.Infrastructure;

public class ExamsDbContext : DbContext
{
    public ExamsDbContext(DbContextOptions<ExamsDbContext> options)
        : base(options)
    {
    }

    public DbSet<ExamPeriod> ExamPeriods => Set<ExamPeriod>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamRegistration> ExamRegistrations => Set<ExamRegistration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Exam>()
            .HasOne(e => e.ExamPeriod)
            .WithMany()
            .HasForeignKey(e => e.ExamPeriodId);

        modelBuilder.Entity<ExamRegistration>()
            .HasIndex(r => new { r.ExamId, r.StudentId })
            .IsUnique();

        modelBuilder.Entity<ExamRegistration>()
            .HasOne(r => r.Exam)
            .WithMany()
            .HasForeignKey(r => r.ExamId);
    }
}
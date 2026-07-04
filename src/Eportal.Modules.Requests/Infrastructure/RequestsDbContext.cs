using Eportal.Modules.Requests.Domain;
using Microsoft.EntityFrameworkCore;

namespace Eportal.Modules.Requests.Infrastructure;

public class RequestsDbContext : DbContext
{
    public RequestsDbContext(DbContextOptions<RequestsDbContext> options)
        : base(options)
    {
    }

    public DbSet<StudentRequest> StudentRequests => Set<StudentRequest>();
}
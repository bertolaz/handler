using Microsoft.EntityFrameworkCore;

namespace Handler.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    
    public DbSet<User> Users { get; set; }
}
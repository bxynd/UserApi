using Microsoft.EntityFrameworkCore;
using UserApi.Entities;

namespace UserApi.Context;

public class DataContext:DbContext
{
    public DbSet<User> Users { get; set; }
    public DataContext()
    {
    }
    public DataContext(DbContextOptions<DataContext> options):base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(entity => {
            entity.HasIndex(e => e.Login).IsUnique();
        });
    }
}
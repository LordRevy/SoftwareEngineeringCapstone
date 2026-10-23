using Microsoft.EntityFrameworkCore;
using AUBE.Objects;

namespace AUBE.Data;

public class AubeDbContext(DbContextOptions<AubeDbContext> options) : DbContext(options)
{
    public DbSet<Unit> Units { get; set; } = null!;
    public DbSet<Weapon> Weapons { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
}

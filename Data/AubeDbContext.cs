using AUBE.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AUBE.Data;

public class AubeDbContext(DbContextOptions<AubeDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Unit> Units { get; set; } = null!;
    public DbSet<Weapon> Weapons { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
}
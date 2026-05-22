using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OzonMarketOrder.Infrastructure;

public class DbContext(DbContextOptions<DbContext> options) : IdentityDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ConfiguringAutomaticIdGeneration(modelBuilder);
        
        modelBuilder.HasDefaultSchema("ozon_market");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);
    }

    private void ConfiguringAutomaticIdGeneration(ModelBuilder modelBuilder)
    {
        var provider = Database.ProviderName;

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var id = entity.FindProperty("Id");
            if (id?.ClrType != typeof(Guid))
                continue;

            id.ValueGenerated = ValueGenerated.OnAdd;

            if (provider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                id.SetDefaultValueSql("gen_random_uuid()");
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class PluginModelConfig : IEntityTypeConfiguration<Plugin>
{
    public void Configure(EntityTypeBuilder<Plugin> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired();
        builder.Property(u => u.PluginName)
            .IsRequired();
       
       
        
        
    }
}
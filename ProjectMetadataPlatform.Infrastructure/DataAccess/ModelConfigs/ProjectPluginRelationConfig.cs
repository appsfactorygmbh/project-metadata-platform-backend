using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class ProjectPluginRelationConfig : IEntityTypeConfiguration<ProjectPlugins>
{
    public void Configure(EntityTypeBuilder<ProjectPlugins> builder)
    {
        builder.Property(e => e.)
            .IsRequired();
        builder.Property(u => u.PluginName)
            .IsRequired();
       
       
        
        
    }
    
}

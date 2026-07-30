using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;

namespace Textile.Core.Infrastructure.EntityConfiguration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Textile.Core.Entities.DbEnitites;

    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table Name
            builder.ToTable("Users");

            // Primary Key
            builder.HasKey(u => u.Id);

            // ID default value (NEWID())
            builder.Property(u => u.Id)
                   .HasDefaultValueSql("NEWID()")
                   .IsRequired();

            // UserName
            builder.Property(u => u.UserName)
                   .HasMaxLength(255)
                   .IsRequired();

            // Password
            builder.Property(u => u.Password)
                   .HasMaxLength(255)
                   .IsRequired();
        }
    }

}

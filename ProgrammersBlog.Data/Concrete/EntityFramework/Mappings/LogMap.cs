using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammersBlog.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Concrete.EntityFramework.Mappings
{
    public class LogMap : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.MachineName).IsRequired();
            builder.Property(x => x.MachineName).HasMaxLength(50);

            builder.Property(x => x.Logged).IsRequired();

            builder.Property(x => x.Level).IsRequired();
            builder.Property(x => x.Level).HasMaxLength(50);

            builder.Property(x => x.Message).IsRequired();
            builder.Property(x => x.Message).HasColumnType("NVARCHAR(MAX)");

            builder.Property(x => x.Logger).IsRequired(false);
            builder.Property(x => x.Logger).HasMaxLength(250);

            builder.Property(x => x.Exception).IsRequired(false);
            builder.Property(x => x.Logger).HasColumnType("NVARCHAR(MAX)");

            builder.ToTable("Logs");

        }
    }
}

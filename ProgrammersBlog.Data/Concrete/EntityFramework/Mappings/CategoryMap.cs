using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammersBlog.Entities.Concrete;
using System;

namespace ProgrammersBlog.Data.Concrete.EntityFramework.Mappings
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.CategoryId);
            builder.Property(c => c.CategoryId).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Name).HasMaxLength(70);
            builder.Property(c => c.Description).HasMaxLength(500);
            builder.Property(c => c.CreatedByName).IsRequired();
            builder.Property(c => c.CreatedByName).HasMaxLength(50);
            builder.Property(c => c.ModifiedByName).IsRequired();
            builder.Property(c => c.ModifiedByName).HasMaxLength(50);
            builder.Property(c => c.CreatedDate).IsRequired();
            builder.Property(c => c.ModifiedDate).IsRequired();
            builder.Property(c => c.IsActive).IsRequired();
            builder.Property(c => c.IsDeleted).IsRequired();
            builder.Property(c => c.Note).HasMaxLength(500);
            builder.ToTable("Categories");
            builder.HasData(
                new Category
                {
                    CategoryId = 1,
                    Description = "C# Programlama dili ile en güncel bilgiler",
                    Name = "C#",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedByName = "InitialCreate",
                    ModifiedByName = "InitialCreate",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Note = "C# Blog Kategorisi"
                },
                 new Category
                 {
                     CategoryId = 2,
                     Description = "C++ Programlama dili ile en güncel bilgiler",
                     Name = "C++",
                     IsActive = true,
                     IsDeleted = false,
                     CreatedByName = "InitialCreate",
                     ModifiedByName = "InitialCreate",
                     CreatedDate = DateTime.Now,
                     ModifiedDate = DateTime.Now,
                     Note = "C++ Blog Kategorisi"
                 },
                  new Category
                  {
                      CategoryId = 3,
                      Description = "JavaScript Programlama dili ile en güncel bilgiler",
                      Name = "JavaScript",
                      IsActive = true,
                      IsDeleted = false,
                      CreatedByName = "InitialCreate",
                      ModifiedByName = "InitialCreate",
                      CreatedDate = DateTime.Now,
                      ModifiedDate = DateTime.Now,
                      Note = "JavaScript Blog Kategorisi"
                  }
                );
        }
    }
}
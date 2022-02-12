using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammersBlog.Entities.Concrete;
using System;

namespace ProgrammersBlog.Data.Concrete.EntityFramework.Mappings
{
    public class ArticleMap : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.HasKey(x => x.ArticleId);
            builder.Property(x => x.ArticleId).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.Content).HasColumnType("NVARCHAR(MAX)");
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.SeoAuthor).IsRequired();
            builder.Property(x => x.SeoAuthor).HasMaxLength(50);
            builder.Property(x => x.SeoDescription).HasMaxLength(150);
            builder.Property(x => x.SeoDescription).IsRequired();
            builder.Property(x => x.SeoTags).IsRequired();
            builder.Property(x => x.SeoTags).HasMaxLength(70);
            builder.Property(x => x.ViewCount).IsRequired();
            builder.Property(x => x.CommentCount).IsRequired();
            builder.Property(x => x.Thumbnail).IsRequired();
            builder.Property(x => x.Thumbnail).HasMaxLength(250);
            builder.Property(x => x.CreatedByName).IsRequired();
            builder.Property(x => x.CreatedByName).HasMaxLength(50);
            builder.Property(x => x.ModifiedByName).IsRequired();
            builder.Property(x => x.ModifiedByName).HasMaxLength(50);
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.ModifiedDate).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.Note).IsRequired();
            builder.HasOne<Category>(x => x.Category).WithMany(x => x.Articles).HasForeignKey(x => x.CategoryId);
            builder.HasOne<User>(x => x.User).WithMany(u => u.Articles).HasForeignKey(x => x.UserId);
            builder.ToTable("Articles");
            //builder.HasData(
            //new Article
            //{
            //    ArticleId = 1,
            //    CategoryId = 1,
            //    Title = "C# 9.0 ve .Net 5.0 yenilikleri",
            //    Content = "Lorem Ipsum, dizgi ve baskı endüstrisinde kullanılan mıgır metinlerdir. Lorem Ipsum, adı bilinmeyen bir matbaacının bir hurufat numune kitabı oluşturmak üzere bir yazı galerisini alarak karıştırdığı 1500'lerden beri endüstri standardı sahte metinler olarak kullanılmıştır. Beşyüz yıl boyunca varlığını sürdürmekle kalmamış, aynı zamanda pek değişmeden elektronik dizgiye de sıçramıştır. 1960'larda Lorem Ipsum pasajları da içeren Letraset yapraklarının yayınlanması ile ve yakın zamanda Aldus PageMaker gibi Lorem Ipsum sürümleri içeren masaüstü yayıncılık yazılımları ile popüler olmuştur.",
            //    Thumbnail = "Default.jpg",
            //    SeoDescription = "C# 9.0 ve .Net 5.0 yenilikleri",
            //    SeoTags = "C#, C# 9, .NET 5.0, .NET Framework, .NET Core",
            //    SeoAuthor = "Bayram Eren",
            //    IsActive = true,
            //    IsDeleted = false,
            //    CreatedByName = "InitialCreate",
            //    ModifiedByName = "InitialCreate",
            //    CreatedDate = DateTime.Now,
            //    ModifiedDate = DateTime.Now,
            //    Note = "C# 9.0 ve .Net 5.0 yenilikleri",
            //    Date = DateTime.Now,
            //    CommentCount = 1,
            //    ViewsCount = 110,
            //    UserId = 1
            //},
            // new Article
            // {
            //     ArticleId = 2,
            //     CategoryId = 2,
            //     Title = "C++ 11 ve 19 yenilikleri",
            //     Content = "Yinelenen bir sayfa içeriğinin okuyucunun dikkatini dağıttığı bilinen bir gerçektir. Lorem Ipsum kullanmanın amacı, sürekli 'buraya metin gelecek, buraya metin gelecek' yazmaya kıyasla daha dengeli bir harf dağılımı sağlayarak okunurluğu artırmasıdır. Şu anda birçok masaüstü yayıncılık paketi ve web sayfa düzenleyicisi, varsayılan mıgır metinler olarak Lorem Ipsum kullanmaktadır. Ayrıca arama motorlarında 'lorem ipsum' anahtar sözcükleri ile arama yapıldığında henüz tasarım aşamasında olan çok sayıda site listelenir. Yıllar içinde, bazen kazara, bazen bilinçli olarak (örneğin mizah katılarak), çeşitli sürümleri geliştirilmiştir.",
            //     Thumbnail = "Default.jpg",
            //     SeoDescription = "C++ 11 ve 19 yenilikleri",
            //     SeoTags = "C++ 11 ve 19 yenilikleri",
            //     SeoAuthor = "Bayram Eren",
            //     IsActive = true,
            //     IsDeleted = false,
            //     CreatedByName = "InitialCreate",
            //     ModifiedByName = "InitialCreate",
            //     CreatedDate = DateTime.Now,
            //     ModifiedDate = DateTime.Now,
            //     Note = "C++ 11 ve 19 yenilikleri",
            //     Date = DateTime.Now,
            //     CommentCount = 1,
            //     ViewsCount = 200,
            //     UserId = 1
            // },
            // new Article
            // {
            //     ArticleId = 3,
            //     CategoryId = 3,
            //     Title = "JavaScript ES2019 ve ES2020 yenilikleri",
            //     Content = "Yaygın inancın tersine, Lorem Ipsum rastgele sözcüklerden oluşmaz. Kökleri M.Ö. 45 tarihinden bu yana klasik Latin edebiyatına kadar uzanan 2000 yıllık bir geçmişi vardır. Virginia'daki Hampden-Sydney College'dan Latince profesörü Richard McClintock, bir Lorem Ipsum pasajında geçen ve anlaşılması en güç sözcüklerden biri olan 'consectetur' sözcüğünün klasik edebiyattaki örneklerini incelediğinde kesin bir kaynağa ulaşmıştır. Lorm Ipsum, Çiçero tarafından M.Ö. 45 tarihinde kaleme alınan \"de Finibus Bonorum et Malorum\" (İyi ve Kötünün Uç Sınırları) eserinin 1.10.32 ve 1.10.33 sayılı bölümlerinden gelmektedir. Bu kitap, ahlak kuramı üzerine bir tezdir ve Rönesans döneminde çok popüler olmuştur. Lorem Ipsum pasajının ilk satırı olan \"Lorem ipsum dolor sit amet\" 1.10.32 sayılı bölümdeki bir satırdan gelmektedir.1500'lerden beri kullanılmakta olan standard Lorem Ipsum metinleri ilgilenenler için yeniden üretilmiştir. Çiçero tarafından        yazılan 1.10.32 ve 1.10.33 bölümleri de 1914 H. Rackham çevirisinden alınan İngilizce sürümleri eşliğinde özgün biçiminden yeniden         üretilmiştir.",
            //     Thumbnail = "Default.jpg",
            //     SeoDescription = "JavaScript ES2019 ve ES2020 yenilikleri",
            //     SeoTags = "JavaScript ES2019 ve ES2020 yenilikleri",
            //     SeoAuthor = "Bayram Eren",
            //     IsActive = true,
            //     IsDeleted = false,
            //     CreatedByName = "InitialCreate",
            //     ModifiedByName = "InitialCreate",
            //     CreatedDate = DateTime.Now,
            //     ModifiedDate = DateTime.Now,
            //     Note = "JavaScript ES2019 ve ES2020 yenilikleri",
            //     Date = DateTime.Now,
            //     CommentCount = 1,
            //     ViewsCount = 12,
            //     UserId = 1
            // }
            //);
        }
    }
}
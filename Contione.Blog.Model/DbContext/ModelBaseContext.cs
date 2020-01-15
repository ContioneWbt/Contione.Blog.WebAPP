using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Contione.Blog.Model.DataModel;
using Contione.Blog.Infrastructure.StaticExtenions;

namespace Contione.Blog.Model.DbContext
{
    public partial class ModelBaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //启用EF延迟加载-可以点出外键对象
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(StaticConstraint.DBContextString);
        }

        public Guid ATest= Guid.NewGuid();//测试ioc容器同一次请求使用的上下文，是不是同一个

        public virtual DbSet<Article> Article { get; set; }

        public virtual DbSet<ArticleType> ArticleType { get; set; }

        public virtual DbSet<Notification> Notification { get; set; }

        public virtual DbSet<QQUser> QQUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(e => e.ID)
                .IsUnicode(false);

                entity.Property(e => e.Title)
                .IsUnicode(false);

                entity.Property(e => e.Img)
                .IsUnicode(false);

                entity.Property(e => e.Href)
                .IsUnicode(false);

                entity.Property(e => e.Abstract)
                .IsUnicode(false);

                entity.Property(e => e.Author)
                .IsFixedLength();

                entity.Property(e => e.Content)
                .IsUnicode(false);
            });

            modelBuilder.Entity<ArticleType>(entity =>
            {
                entity.Property(e => e.Name)
               .IsUnicode(false);

                entity.Property(e => e.Id)
                .IsUnicode(false);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Color)
                .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsUnicode(false);

                entity.Property(e => e.Link)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<QQUser>(entity =>
            {
                entity.Property(e => e.Id)
               .IsUnicode(false);

                entity.Property(e => e.QQOpenId)
                .IsUnicode(false);

                entity.Property(e => e.Name)
                .IsUnicode(false);
            });
        }
    }
}

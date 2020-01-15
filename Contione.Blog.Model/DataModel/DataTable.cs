
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Contione.Blog.Model;

#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning
namespace Contione.Model.DataModel
{
    using System;
    using System.Linq;
    using Contione.Model;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;

	#region POCO classes

    // SysCompany
    ///<summary>
    /// 服务器大区
    ///</summary>
    [Table("Author")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.30.0.0")]
    public class Author : AggregateRoot
    {
        public Guid PKId { get; set; }
        public int Id { get; set; }
        //作者姓名
        public string AuthorName { get; set; }

        public virtual System.Collections.Generic.ICollection<Children> Childrens { get; set; } // SysDept.FK_SysDept_SYSCOMPAN_SysCompany
    }

    [Table("Children")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.30.0.0")]
    public class Children : AggregateRoot
    {
        public Guid id { get; set; }
        public Guid childrenid { get; set; }
        //作者姓名
        public string Name { get; set; }

        public virtual Author Author { get; set; } // FK_SysDept_SYSCOMPAN_SysCompany
    }
    #endregion

    #region POCO Configuration
    // SysTitle
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.30.0.0")]
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Author").HasKey(r => r.PKId); //注：如果Author这个表是以Id做主键的，则可以不用写HasKey(r=>r.Id);
            builder.Property(r => r.Id).HasMaxLength(20);
            builder.Property(r => r.AuthorName).HasMaxLength(20);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.30.0.0")]
    public class ChildrenConfiguration : IEntityTypeConfiguration<Children>
    {
        public void Configure(EntityTypeBuilder<Children> builder)
        {
            builder.ToTable("Children").HasKey(r => r.id); //注：如果Author这个表是以Id做主键的，则可以不用写HasKey(r=>r.Id);
            builder.Property(r => r.childrenid).HasColumnType("uniqueidentifier"); 
            builder.Property(r => r.Name).HasMaxLength(20);

            builder.HasOne(f => f.Author).WithMany(p => p.Childrens).HasForeignKey(p => p.childrenid);
        }
    }
    #endregion
}
// </auto-generated>

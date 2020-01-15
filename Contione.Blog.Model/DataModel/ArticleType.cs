using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contione.Blog.Model.DataModel
{
    [Table("ArticleType")]
    public partial class ArticleType : AggregateRoot
    {
        [Key]
        public int TypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Id { get; set; }
    }
}

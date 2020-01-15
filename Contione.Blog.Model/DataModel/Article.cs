using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Contione.Blog.Model.DataModel
{
    [Table("Article")]
    public partial  class Article : AggregateRoot
    {
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(1000)]
        public string Title { get; set; }

        [StringLength(220)]
        public string Img { get; set; }

        [StringLength(50)]
        public string Href { get; set; }

        [StringLength(2000)]
        public string Abstract { get; set; }

        public int? TitleType { get; set; }

        public DateTime CreateTime { get; set; }

        [Required]
        [StringLength(10)]
        public string Author { get; set; }

        public int? ArticleType { get; set; }

        public int? ViewNumber { get; set; }

        public int? CommentNumber { get; set; }

        public int? Sort { get; set; }

        public string Content { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdentityId { get; set; }
    }
}

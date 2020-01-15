using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Contione.Blog.Model.DataModel
{
    [Table("QQUser")]
    public partial class QQUser : AggregateRoot
    {
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string QQOpenId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Sign { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Remark { get; set; }

        public bool? Status { get; set; }

        [StringLength(100)]
        public string Avatar { get; set; }

        [StringLength(100)]
        public string LastLoginIP { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreateTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? LastLoginTime { get; set; }
    }
}

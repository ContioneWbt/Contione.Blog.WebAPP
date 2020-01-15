using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Contione.Blog.Model.DataModel
{
    [Table("Notification")]
    public partial class Notification : AggregateRoot
    {
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(10)]
        public string Color { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        public bool? IsOpen { get; set; }

        [StringLength(500)]
        public string Link { get; set; }
    }
}

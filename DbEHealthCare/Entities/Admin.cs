namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Admin")]
    public partial class Admin
    {
        [Key]
        [StringLength(200)]
        public string email { get; set; }

        [StringLength(100)]
        public string Pass { get; set; }

        [StringLength(100)]
        public string Username { get; set; }
    }
}

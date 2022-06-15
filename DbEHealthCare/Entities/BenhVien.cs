namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BenhVien")]
    public partial class BenhVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BenhVien()
        {
            BacSis = new HashSet<BacSi>();
        }

        [StringLength(6)]
        public string id { get; set; }

        [StringLength(100)]
        public string ten_bv { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BacSi> BacSis { get; set; }
    }
}

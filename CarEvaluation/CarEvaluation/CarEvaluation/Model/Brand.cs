using System;
using System.Collections.Generic;
using System.Text;

namespace CarEvaluation
{
    public class Brand
    {
        public int idBrand { get; set; }
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Model> Model { get; set; }
    }
}

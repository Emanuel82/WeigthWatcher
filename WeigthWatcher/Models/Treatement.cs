//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WeigthWatcher.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Treatement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Treatement()
        {
            this.Treatement2Patient = new HashSet<Treatement2Patient>();
        }
    
        public int TreatementId { get; set; }
        public int PatientId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
    
        public virtual Treatement Treatement1 { get; set; }
        public virtual Treatement Treatement2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Treatement2Patient> Treatement2Patient { get; set; }
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AgroBoard.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeviceAndSensor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeviceAndSensor()
        {
            this.Status = new HashSet<Status>();
            this.Telemetry = new HashSet<Telemetry>();
        }
    
        public string Name { get; set; }
        public string description { get; set; }
        public string Protocol { get; set; }
        public Nullable<int> aId { get; set; }
        public string Type { get; set; }
    
        public virtual Area Area { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Status> Status { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Telemetry> Telemetry { get; set; }
    }
}

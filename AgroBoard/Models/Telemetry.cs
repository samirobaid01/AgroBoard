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
    
    public partial class Telemetry
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Telemetry()
        {
            this.TelemetryData = new HashSet<TelemetryData>();
        }
    
        public int Id { get; set; }
        public string Variable { get; set; }
        public string DataType { get; set; }
        public string DeviceName { get; set; }
    
        public virtual DeviceAndSensor DeviceAndSensor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TelemetryData> TelemetryData { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyEverNote.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Category : EntityBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            this.Note = new HashSet<Note>();
        }
    
        public int Id { get; set; }
        [Required, MaxLength(20, ErrorMessage = "Title alan� maximum {1} karakter olmal�d�r.")]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string ModifiedUser { get; set; }
        public System.DateTime CreateOn { get; set; }
        public System.DateTime ModifieOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Note { get; set; }
    }
}

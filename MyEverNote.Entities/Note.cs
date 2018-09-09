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

    public partial class Note : EntityBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Note()
        {
            this.Comment = new HashSet<Comment>();
            this.NoteLike = new HashSet<NoteLike>();
        }
    
        public int Id { get; set; }
        [Required, MaxLength(30, ErrorMessage = "Title alan� maximum {1} karakter olmal�d�r.")]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public int LikeCount { get; set; }
        [Required(ErrorMessage = "Category alan� gereklidir.")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage ="User alan� gereklidir.")]
        public int OwnerId { get; set; }
        public string ModifiedUser { get; set; }
        public System.DateTime ModifieOn { get; set; }
        public System.DateTime CreateOn { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual EverNoteUser EverNoteUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoteLike> NoteLike { get; set; }
    }
}
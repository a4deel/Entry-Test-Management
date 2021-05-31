//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntryTestManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public partial class UserData
    {
        public int id { get; set; }

        public string email { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Contact is Required")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "CNIC is Required")]
        public Nullable<decimal> CNIC { get; set; }

        public string Image { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Matric marks are Required")]
        public string Matric { get; set; }

        [Required(ErrorMessage = "FSC marks are Required")]
        public string Inter { get; set; }

        [Required(ErrorMessage = "Age is Required")]
        public string Age { get; set; }

        public string Status { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
    }
}

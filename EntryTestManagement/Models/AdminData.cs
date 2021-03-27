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

    public partial class AdminData
    {
        [Required(ErrorMessage = "Email is Required")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Format")]
        public string email { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Contact is Required")]
        public string Contact { get; set; }

        public string Image { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "CNIC is Required")]
        public Nullable<decimal> CNIC { get; set; }
        public int id { get; set; }
        public string Role { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
}

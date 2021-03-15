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
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class UserLogin
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",ErrorMessage ="Email format is Invalid")]
        public string email { get; set; }

        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$",ErrorMessage ="Password Req : Min Length 8,at least 1 number and letter")]
        [Required(ErrorMessage = "Password is Required")]
        public string password { get; set; }

        [NotMapped]
        [Required(ErrorMessage ="Confirm Password is Required")]
        [Compare("password")]
        public string ConfirmPassword { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace statenet_lspd.Models
{
    public class EditProfileViewModel
    {
        [Display(Name = "Geburtstag")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Telefon")]
        [Phone]
        public string? Phone { get; set; }
    }

}
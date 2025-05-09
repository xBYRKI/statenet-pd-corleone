using System;
using System.ComponentModel.DataAnnotations;

namespace statenet_lspd.Models
{
    public class HireWizardViewModel
    {
        [Required]
        public Step1Model Step1 { get; set; } = new Step1Model();

        [Required]
        public Step2Model Step2 { get; set; } = new Step2Model();

        [Required]
        public Step3Model Step3 { get; set; } = new Step3Model();
    }

    public class Step1Model
    {
        [Required, Display(Name = "Vor- und Nachname")]
        public string FullName { get; set; } = string.Empty;

        [Required, Display(Name = "Dienstnummer")]
        public int? Dienstnummer { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Geburtsdatum")]
        public DateTime? Birthday { get; set; }

        [Phone, Display(Name = "Telefonnummer")]
        public string? Phone { get; set; }
    }

    public class Step2Model
    {
        [Required, Display(Name = "Rang")]
        public int? RankId { get; set; }

        [Required, Display(Name = "Besoldungsstufe")]
        public int? Paygrade { get; set; }

        [Display(Name = "Grund (wenn über MaxPaygrade)")]
        public string? PaygradeReason { get; set; }
    }

    public class Step3Model
    {
        [Required, Display(Name = "CopNet Daten angelegt und versendet?")]
        public bool CopNetDone { get; set; }

        [Required, Display(Name = "Einweisung durchgeführt?")]
        public bool InstructionDone { get; set; }
    }
}
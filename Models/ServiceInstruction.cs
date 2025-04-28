using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace statenet_lspd.Models
{
    public class ServiceInstruction
    {
        public int Id { get; set; }
        public string Title { get; set; }                // z.B. „IT-Sicherheitsrichtlinie“
        public string ContentHtml { get; set; }          // mit HTML-Markup
        public DateTime EffectiveDate { get; set; }      // ab wann gültig
        public bool IsActive { get; set; }               // alte Anweisungen können deaktiviert werden
    }
}
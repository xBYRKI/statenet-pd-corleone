using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace statenet_lspd.Models
{
    public enum Permission
    {
        [Display(Name = "HR anzeigen")]
        HR_View,

        [Display(Name = "Anlegen")]
        HR_Create,

        [Display(Name = "Kündigen")]
        HR_Delete,

        [Display(Name = "Sanktionieren")]
        HR_Sanction,

        [Display(Name = "Beförderen")]
        HR_Promotion,

        [Display(Name = "Degradieren")]
        HR_Demotion,

        [Display(Name = "Suspendieren")]
        HR_Suspension,

        [Display(Name = "Rollenübersicht anzeigen")]
        ROLE_View,

        [Display(Name = "Rolle hinzufügen")]
        ROLE_Add,

        [Display(Name = "Rolle bearbeiten")]
        ROLE_Edit,

        [Display(Name = "Rolle löschen")]
        ROLE_Delete,

        [Display(Name = "Rollenberechtigung sehen")]
        RolesPerm_View,

        [Display(Name = "Rollenberechtigung bearbeiten")]
        RolesPerm_Update,

        [Display(Name = "Besoldungsstufen anzeigen")]
        Paygrade_View,

        [Display(Name = "Besoldungsstufe anlegen")]
        Paygrade_Create,

        [Display(Name = "Besoldungsstufe bearbeiten")]
        Paygrade_Edit,

        [Display(Name = "Besoldungsstufe löschen")]
        Paygrade_Delete,

        [Display(Name = "Dienstanweisungen verwalten")]
        Instructions_View,

        [Display(Name = "Dienstanweisungen erstellen")]
        Instructions_Add,

        [Display(Name = "Dienstanweisungen bearbeiten")]
        Instructions_Edit,

        [Display(Name = "Dienstanweisungen löschen")]
        Instructions_Delete,
        // weitere Permissions...
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();
            if (member == null)
                return value.ToString();

            var displayAttr = member
                .GetCustomAttribute<DisplayAttribute>(false);

            return displayAttr?.GetName() ?? value.ToString();
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Internship_3_OOP
{
    internal enum EmployeePosition
    {
        [Display(Name = "Recepcionist(ica)")]
        Receptionist,
        [Display(Name = "Sobar(ica)")]
        Steward,
        [Display(Name = "Kuhar(ica)")]
        Cook
    }
}

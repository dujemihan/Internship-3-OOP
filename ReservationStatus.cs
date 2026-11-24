using System.ComponentModel.DataAnnotations;

namespace Internship_3_OOP
{
    internal enum ReservationStatus
    {
        [Display(Name = "Potvrđena")]
        Confirmed,
        [Display(Name = "Poništena")]
        Canceled,
        [Display(Name = "Završena")]
        Finished
    }
}

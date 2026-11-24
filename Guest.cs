using System.Diagnostics.CodeAnalysis;

namespace Internship_3_OOP
{
    internal sealed class Guest : IAuditable
    {
        private static readonly List<Guest> _guests = [];
        public Guid Guid { get; init; } = Guid.NewGuid();
        public DateTime CreationTime { get; init; } = DateTime.Now;
        public DateTime ModificationTime { get; set; } = DateTime.Now;
        public string Name { get; private init; }
        public string Surname { get; private init; }
        private int BirthYear { get; init; }
        private string Email { get; init; }
        private string Password { get; init; }

        public Guest(string name, string surname, int birthYear, string email, string password)
        {
            if (!IsEmailUnique(email))
                throw new ArgumentException("Email nije jedinstven.");

            if (password.Length < 6)
                throw new ArgumentException("Lozinka je prekratka.");

            if (birthYear > DateTime.Today.Year)
                throw new ArgumentException("Godina rođenja ne može biti u budućnosti.");

            Name = name;
            Surname = surname;
            BirthYear = birthYear;
            Email = email;
            Password = password;

            _guests.Add(this);
        }

        public static bool TryLogin(string email, string password, [NotNullWhen(true)] out Guest? guest)
        {
            guest = _guests.FirstOrDefault(g => g.Email == email && g.Password == password);
            return guest != default;
        }

        public bool HasReservation(DateTime start, DateTime end) =>
            Reservation._reservations.Any(r =>
                r.GuestGuid == Guid &&
                start < r.CheckOutDate &&
                end > r.CheckInDate);

        private static bool IsEmailUnique(string email) =>
            !_guests.Any(g => g.Email == email);
    }
}

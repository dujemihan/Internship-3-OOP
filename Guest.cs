namespace Internship_3_OOP
{
    internal sealed class Guest : IAuditable
    {
        private static readonly List<Guest> _guests = [];
        public Guid Guid { get; init; } = Guid.NewGuid();
        public DateTime CreationTime { get; init; } = DateTime.Now;
        public DateTime ModificationTime { get; set; } = DateTime.Now;
        private string Name { get; init; }
        private string Surname { get; init; }
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

        public static bool TryLogin(string email, string password, out Guest? guest)
        {
            guest = _guests.FirstOrDefault(g => g.Email == email && g.Password == password);
            return guest != default;
        }

        private static bool IsEmailUnique(string email) =>
            _guests.Find(g => g.Email == email) == default;
    }
}

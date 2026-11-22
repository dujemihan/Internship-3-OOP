namespace Internship_3_OOP
{
    internal sealed class Guest
    {
        private static readonly List<Guest> _guests = [];
        private string Name { get; set; }
        private string Surname { get; set; }
        private int BirthYear { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }

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

        private static bool IsEmailUnique(string email)
        {
            return _guests.Find(g => g.Email == email) == default;
        }
    }
}

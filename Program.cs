using System.Text;

namespace Internship_3_OOP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            _ = new Guest("Ivica", "Horvat", 1990, "ivan.h@gmail.com", "test1234");
            _ = new Guest("Mate", "Matić", 1990, "maticmate@gmail.com", "4321tset");
            _ = new Guest("Franjo", "Franić", 1990, "ffranjo@gmail.com", "defg9876");

            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Gosti", ShowGuestsMenu),
                ['0'] = new("Izlaz iz programa", () => MenuAction.Exit)
            });

            while (menu.Show() == MenuAction.Stay) ;
        }

        static MenuAction ShowGuestsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Registracija", RegisterNewGuest),
                ['2'] = new("Prijava", () => MenuAction.Stay),
                ['0'] = new("Povratak", () => MenuAction.Exit)
            });

            while (true)
            {
                var action = menu.Show();
                switch (action)
                {
                    case MenuAction.PauseAndStay:
                        Menu.WaitForKey();
                        break;
                    default:
                        return action;
                }
            }
        }

        static MenuAction RegisterNewGuest()
        {
            Console.Clear();

            var name = Menu.ReadRequiredString("Unesite ime", "Molimo unesite ime!");
            var surname = Menu.ReadRequiredString("Unesite prezime", "Molimo unesite prezime!");
            var birthYear = Menu.ReadRequiredInt("Unesite godinu rođenja", "Molimo unesite godinu!");
            var email = Menu.ReadRequiredString("Unesite adresu e-pošte", "Molimo unesite adresu!");
            var password = Menu.ReadRequiredString("Unesite lozinku", "Molimo unesite lozinku!");

            if (!Menu.IsPromptPositive("Želite li se sigurno registrirati?"))
                return MenuAction.Stay;

            try
            {
                _ = new Guest(name, surname, birthYear, email, password);
                Console.WriteLine("Gost uspješno registriran.");
            } catch (Exception ex)
            {
                Console.WriteLine($"Došlo je do greške pri registraciji: {ex.Message}");
            }

            return MenuAction.PauseAndStay;
        }
    }
}

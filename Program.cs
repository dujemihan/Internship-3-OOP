using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace Internship_3_OOP
{
    internal class Program
    {
        public static readonly int GUID_CHARACTER_COUNT = 8;

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            _ = new Guest("Ivica", "Horvat", 1990, "ivan.h@gmail.com", "test1234");
            _ = new Guest("Mate", "Matić", 1990, "maticmate@gmail.com", "4321tset");
            _ = new Guest("Franjo", "Franić", 1990, "ffranjo@gmail.com", "defg9876");

            _ = new Room(101, 1, RoomCategory.Standard, 3, 50.0m);
            _ = new Room(206, 7, RoomCategory.Deluxe, 2, 90.0m);
            _ = new Room(153, 3, RoomCategory.Standard, 2, 40.0m);
            _ = new Room(189, 5, RoomCategory.Standard, 4, 65.0m);
            _ = new Room(301, 9, RoomCategory.Suite, 1, 180.0m);

            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Gosti", ShowGuestsMenu),
                ['3'] = new("Sobe", ShowRoomsMenu),
                ['0'] = new("Izlaz iz programa", () => MenuAction.Exit)
            });

            menu.Run();
        }

        static MenuAction ShowGuestsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Registracija", RegisterNewGuest),
                ['2'] = new("Prijava", ShowGuestMenu),
                ['0'] = new("Povratak", () => MenuAction.Exit)
            });

            return menu.Run();
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

        static MenuAction ShowGuestMenu()
        {
            var email = Menu.ReadRequiredString("Unesite adresu e-pošte", "Molimo unesite adresu!");
            var password = Menu.ReadRequiredString("Unesite lozinku", "Molimo unesite lozinku!");
            if (!Guest.TryLogin(email, password, out var guest))
            {
                Console.WriteLine("Prijava nije uspjela, molimo provjerite podatke i pokušajte ponovno.");
                return MenuAction.PauseAndStay;
            }

            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                {'1', new("Moje rezervacije", PrintGuestReservations)},
                {'2', new("Nova rezervacija", () => throw new NotImplementedException())},
                {'3', new("Pretraživanje soba", () => throw new NotImplementedException())},
                {'4', new("Otkazivanje rezervacije", () => throw new NotImplementedException())},
                {'5', new("Povratak na prethodni izbornik", () => MenuAction.Exit)}
            });

            return menu.Run(new MenuContext
            {
                Guest = guest
            });
        }

        static MenuAction PrintGuestReservations(MenuContext? context)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(context.Guest);

            Console.Clear();

            foreach (var reservation in Reservation.GetGuestReservations(context.Guest))
            {
                Console.WriteLine(reservation);
            }

            return MenuAction.PauseAndStay;
        }

        static MenuAction ShowRoomsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                {'1', new MenuItem("Prikaz svih soba", ShowAllRooms)},
                {'5', new MenuItem("Povratak na prethodni izbornik", () => MenuAction.Exit)}
            });

            return menu.Run();
        }

        static MenuAction ShowAllRooms()
        {
            foreach (var room in Room.AllRooms)
                Console.WriteLine(room);

            return MenuAction.PauseAndStay;
        }
    }
}

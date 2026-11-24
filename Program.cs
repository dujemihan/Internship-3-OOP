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

            InitializeTestData();

            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Gosti", ShowGuestsMenu),
                ['2'] = new("Rezervacije", ShowReservationsMenu),
                ['3'] = new("Sobe", ShowRoomsMenu),
                ['0'] = new("Izlaz iz programa", () => MenuAction.Exit)
            }, "PROGRAM ZA VOĐENJE HOTELA");

            menu.Run();
        }

        static MenuAction ShowGuestsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Registracija", RegisterNewGuest),
                ['2'] = new("Prijava", ShowGuestMenu),
                ['0'] = new("Povratak", () => MenuAction.Exit)
            }, "GOSTI");

            return menu.Run();
        }

        static MenuAction RegisterNewGuest()
        {
            Console.Clear();

            var name = Menu.ReadRequiredString("Unesite ime", "Molimo unesite ime!");
            var surname = Menu.ReadRequiredString("Unesite prezime", "Molimo unesite prezime!");
            var birthYear = Menu.ReadRequired<int>("Unesite godinu rođenja", "Molimo unesite godinu!");
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
            }, $"GOST: {guest.Surname}, {guest.Name}");

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

            foreach (var reservation in Reservation._reservations.Where(r => r.GuestGuid == context.Guest.Guid))
                Console.WriteLine(reservation);

            return MenuAction.PauseAndStay;
        }

        static MenuAction ShowReservationsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Prikaz svih rezervacija", ShowAllReservations),
                ['6'] = new("Povratak na prethodni izbornik", () => MenuAction.Exit)
            }, "REZERVACIJE");

            return menu.Run();
        }

        static MenuAction ShowAllReservations()
        {
            Console.Clear();

            foreach (var reservation in Reservation._reservations)
                Console.WriteLine(reservation);

            return MenuAction.PauseAndStay;
        }

        static MenuAction ShowRoomsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                {'1', new MenuItem("Prikaz svih soba", ShowAllRooms)},
                {'2', new MenuItem("Dodavanje nove sobe", AddNewRoom)},
                {'3', new MenuItem("Pretraživanje soba", SearchRooms)},
                {'4', new MenuItem("Brisanje sobe", DeleteRoom)},
                {'5', new MenuItem("Povratak na prethodni izbornik", () => MenuAction.Exit)}
            }, "SOBE");

            return menu.Run();
        }

        static MenuAction ShowAllRooms()
        {
            foreach (var room in Room._rooms)
                Console.WriteLine(room);

            return MenuAction.PauseAndStay;
        }

        static MenuAction AddNewRoom()
        {
            Console.Clear();

            var roomNumber = Menu.ReadOptional<int>("Unesite broj sobe (prazno za odustajanje)");
            if (roomNumber == default)
                return MenuAction.PauseAndStay;

            var floorNumber = Menu.ReadRequired<int>("Unesite broj kata", "Molimo unesite valjan broj kata!");
            RoomCategory category;
            while (!Enum.TryParse(Menu.ReadRequiredString("Unesite kategoriju sobe", "Molimo unesite valjanu kategoriju!"),
                true, out category)) ;
            var capacity = Menu.ReadRequired<int>("Unesite kapacitet sobe (broj osoba)", "Molimo unesite kapacitet!");
            var nightPrice = Menu.ReadRequired<decimal>("Unesite cijenu noćenja", "Molimo unesite cijenu!");

            if (Menu.IsPromptPositive("Želite li dodati sobu?"))
            {
                try
                {
                    _ = new Room(roomNumber, floorNumber, category, capacity, nightPrice);
                    Console.WriteLine("Soba uspješno dodana.");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Greška pri dodavanju sobe: {ex.Message}");
                }
            }

            return MenuAction.PauseAndStay;
        }

        static MenuAction SearchRooms()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                {'1', new("Po broju", SearchRoomsByNumber)},
                {'2', new("Po kategoriji", SearchRoomsByCategory)},
                {'3', new("Povratak na prethodni izbornik", () => MenuAction.Exit)},
            }, "PRETRAŽIVANJE SOBA");

            return menu.Run();
        }

        static MenuAction SearchRoomsByNumber()
        {
            var number = Menu.ReadOptional<int>("Unesite broj sobe (prazno za odustajanje)");
            if (number == default)
                return MenuAction.Stay;

            var room = Room._rooms.FirstOrDefault(r => r.RoomNumber == number);
            if (room == default)
            {
                Console.WriteLine("Soba nije pronađena.");
                return MenuAction.PauseAndStay;
            }

            Console.WriteLine(room);
            return MenuAction.PauseAndStay;
        }

        static MenuAction SearchRoomsByCategory()
        {
            Console.Clear();
            Console.WriteLine($"Moguće kategorije: {string.Join(',', Enum.GetValues<RoomCategory>())}");

            if (!Enum.TryParse(Menu.ReadOptionalString("Unesite kategoriju sobe (prazno za odustajanje)"),
                true, out RoomCategory category))
                return MenuAction.Stay;

            foreach (var room in Room._rooms.Where(r => r.Category == category))
                Console.WriteLine(room);

            return MenuAction.PauseAndStay;
        }

        static MenuAction DeleteRoom()
        {
            Console.Clear();
            ShowAllRooms();
            Console.WriteLine();

            var targetGuid = Menu.ReadOptionalString("Unesite ID sobe (prazno za odustajanje)");
            if (targetGuid is null)
                return MenuAction.PauseAndStay;

            Console.WriteLine();    
            var target = Room._rooms.FirstOrDefault(
                r => r.Guid.ToString()[..GUID_CHARACTER_COUNT].SequenceEqual(targetGuid));
            if (target == default)
            {
                Console.WriteLine("Ta soba ne postoji.");
                return MenuAction.PauseAndStay;
            }

            Console.WriteLine(target);
            if (Menu.IsPromptPositive("Želite li obrisati ovu sobu?"))
            {
                if (Room._rooms.Remove(target))
                    Console.WriteLine("Soba uspješno izbrisana.");
                else
                    Console.WriteLine("Došlo je do nepoznate greške pri brisanju!");
            }

            return MenuAction.PauseAndStay;
        }

        static void InitializeTestData()
        {
            var IvicaH = new Guest("Ivica", "Horvat", 1990, "ivan.h@gmail.com", "test1234");
            var MateM = new Guest("Mate", "Matić", 1990, "maticmate@gmail.com", "4321tset");
            var FranjoF = new Guest("Franjo", "Franić", 1990, "ffranjo@gmail.com", "defg9876");

            var RoomA = new Room(101, 1, RoomCategory.Standard, 3, 50.0m);
            var RoomB = new Room(206, 7, RoomCategory.Deluxe, 2, 90.0m);
            var RoomC = new Room(153, 3, RoomCategory.Standard, 2, 40.0m);
            var RoomD = new Room(189, 5, RoomCategory.Standard, 4, 65.0m);
            var RoomE = new Room(301, 9, RoomCategory.Suite, 1, 180.0m);

            _ = new Reservation(IvicaH, RoomA, new(2026, 8, 10), new(2026, 8, 17));
            _ = new Reservation(FranjoF, RoomC, new(2026, 8, 5), new(2026, 8, 8));
            _ = new Reservation(MateM, RoomE, new(2026, 8, 12), new(2026, 8, 15));
            _ = new Reservation(FranjoF, RoomB, new(2026, 8, 3), new(2026, 8, 5));
            _ = new Reservation(IvicaH, RoomD, new(2026, 8, 1), new(2026, 8, 5));
        }
    }
}

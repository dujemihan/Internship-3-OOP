using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Internship_3_OOP
{
    internal static class Program
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
                ['4'] = new("Osoblje", ShowPersonnelMenu),
                ['5'] = new("Izlaz iz programa", () => MenuAction.Exit)
            }, "PROGRAM ZA VOĐENJE HOTELA");

            menu.Run();
        }

        static MenuAction ShowGuestsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Registracija", RegisterNewGuest),
                ['2'] = new("Prijava", ShowGuestMenu),
                ['3'] = new("Povratak", () => MenuAction.Exit)
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
                ['1'] = new("Moje rezervacije", PrintGuestReservations),
                ['2'] = new("Nova rezervacija", AddNewReservation),
                ['3'] = new("Pretraživanje soba", SearchRooms),
                ['4'] = new("Otkazivanje rezervacije", CancelReservation),
                ['5'] = new("Povratak na prethodni izbornik", () => MenuAction.Exit)
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

        static MenuAction CancelReservation([NotNull] MenuContext? context)
        {
            Console.Clear();

            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(context.Guest);

            var reservations = Reservation._reservations.Where(
                r => r.GuestGuid == context.Guest.Guid &&
                r.Status == ReservationStatus.Confirmed &&
                r.CheckInDate > DateTime.Now.AddDays(2)).ToList();
            if (reservations.Count == 0)
            {
                Console.WriteLine("Nemate rezervacija koje se mogu poništiti.");
                return MenuAction.PauseAndStay;
            }

            var reservation = Menu.SelectFromList(reservations);
            if (Menu.IsPromptPositive("Želite li poništiti rezervaciju?"))
            {
                try
                {
                    var index = Reservation._reservations.IndexOf(reservation);
                    reservation.Status = ReservationStatus.Canceled;
                    Reservation._reservations[index] = reservation;
                    Console.WriteLine("Rezervacija uspješno otkazana.");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Došlo je do greške: {ex.Message}");
                }
            }

            return MenuAction.PauseAndStay;
        }

        static MenuAction ShowReservationsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Prikaz svih rezervacija", ShowAllReservations),
                ['2'] = new("Dodavanje rezervacije", AddNewReservation),
                ['3'] = new("Pretraživanje rezervacija", SearchReservations),
                ['4'] = new("Uređivanje rezervacija", EditReservation),
                ['5'] = new("Brisanje rezervacije", DeleteReservation),
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

        static MenuAction AddNewReservation(MenuContext? context = null)
        {
            bool HasPassedGuest = context is not null;
            Guest? guest = context?.Guest;
            Console.Clear();

            if (!HasPassedGuest)
                guest = Menu.SelectFromList(Guest._guests);

            ArgumentNullException.ThrowIfNull(guest);

            var checkIn = Menu.ReadRequired<DateTime>("Unesite datum prijave", "Molimo unesite datum!");
            var checkOut = Menu.ReadRequired<DateTime>("Unesite datum odjave", "Molimo unesite datum!");

            if (guest.HasReservation(checkIn, checkOut))
            {
                Console.WriteLine($"{(HasPassedGuest ? "Već imate" : "Gost već ima")} rezervaciju u tom periodu!");
                return MenuAction.PauseAndStay;
            }

            var availRooms = Room._rooms.Where(
                r => !r.IsOccupied(checkIn, checkOut)).ToList();

            if (availRooms.Count == 0)
            {
                Console.WriteLine("Nema slobodnih soba u tom periodu.");
                return MenuAction.PauseAndStay;
            }

            var room = Menu.SelectFromList(availRooms);

            if (Menu.IsPromptPositive("Želite li stvoriti rezervaciju"))
            {
                try
                {
                    _ = new Reservation(guest, room, checkIn, checkOut);
                    Console.WriteLine("Rezervacija uspješno stvorena.");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Došlo je do greške: {ex.Message}");
                }
            }

            return MenuAction.PauseAndStay;
        }

        static MenuAction SearchReservations()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Po ID-ju", SearchReservationsById),
                ['2'] = new("Po prezimenu gosta", SearchReservationsBySurname)
            });

            return menu.Show();
        }

        static MenuAction SearchReservationsById()
        {
            Console.Clear();

            var guid = Menu.ReadOptionalString("Unesite ID (prazno = odustani)");
            if (string.IsNullOrWhiteSpace(guid))
                return MenuAction.Stay;

            var reservation = Reservation._reservations.FirstOrDefault(
                r => r.Guid.ToString()[..GUID_CHARACTER_COUNT] == guid);
            if (reservation == default)
            {
                Console.WriteLine("Ta rezervacija ne postoji.");
                return MenuAction.PauseAndStay;
            }

            Console.WriteLine(reservation);

            return MenuAction.PauseAndStay;
        }

        static MenuAction SearchReservationsBySurname()
        {
            Console.Clear();

            var surname = Menu.ReadOptionalString("Unesite prezime (prazno = odustani)");
            if (string.IsNullOrWhiteSpace(surname))
                return MenuAction.Stay;

            var guest = Guest._guests.FirstOrDefault(g => g.Surname.Equals(surname, StringComparison.CurrentCultureIgnoreCase));
            if (guest == default)
            {
                Console.WriteLine("Taj gost ne postoji.");
                return MenuAction.PauseAndStay;
            }

            foreach (var reservation in Reservation._reservations.Where(r => r.GuestGuid == guest.Guid))
                Console.WriteLine(reservation);

            return MenuAction.PauseAndStay;
        }

        static MenuAction EditReservation()
        {
            Console.Clear();

            var reservation = Menu.SelectFromList(Reservation._reservations);
            var room = Room._rooms.FirstOrDefault(r => r.Guid == reservation.RoomGuid);
            ArgumentNullException.ThrowIfNull(room);

            DateTime checkIn, checkOut;
            while (true)
            {
                checkIn = Menu.ReadRequiredMatching<DateTime>("Unesite novi datum prijave",
                    dt => dt >= DateTime.Now, "Prijava mora biti u budućnosti!");
                checkOut = Menu.ReadRequiredMatching<DateTime>("Unesite novi datum odjave",
                    dt => dt > checkIn, null, "Odjava mora biti nakon prijave!");

                if (room.IsOccupied(checkIn, checkOut))
                    Console.WriteLine("Soba je zauzeta u tom rasponu!");
                else
                    break;
            }

            if (Menu.IsPromptPositive("Želite li urediti rezervaciju?"))
            {
                try
                {
                    ReplaceEntity(Reservation._reservations, reservation,
                    reservation =>
                    {
                        reservation.CheckInDate = checkIn;
                        reservation.CheckOutDate = checkOut;
                        return reservation;
                    });
                    Console.WriteLine("Rezervacija uspješno promijenjena.");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Došlo je do greške: {ex.Message}");
                }
            }

            return MenuAction.PauseAndStay;
        }

        static MenuAction DeleteReservation()
        {
            Console.Clear();

            ShowAllReservations();
            Console.WriteLine();

            var guid = Menu.ReadOptionalString("Unesite ID rezervacije za brisanje (prazno = odustani)");
            if (string.IsNullOrEmpty(guid))
                return MenuAction.PauseAndStay;

            var target = Reservation._reservations.FirstOrDefault(
                r => r.Guid.ToString()[..Program.GUID_CHARACTER_COUNT] == guid);
            if (target == default)
            {
                Console.WriteLine("Ta rezervacija ne postoji.");
                return MenuAction.PauseAndStay;
            }

            Console.WriteLine(target);
            if (Menu.IsPromptPositive("Želite li obrisati ovu rezervaciju?"))
            {
                if (Reservation._reservations.Remove(target))
                    Console.WriteLine("Rezervacija uspješno obrisana.");
                else
                    Console.WriteLine("Došlo je do greške pri brisanju!");

                return MenuAction.PauseAndStay;
            }
            else
                return MenuAction.Stay;
        }

        static MenuAction ShowRoomsMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new MenuItem("Prikaz svih soba", ShowAllRooms),
                ['2'] = new MenuItem("Dodavanje nove sobe", AddNewRoom),
                ['3'] = new MenuItem("Pretraživanje soba", SearchRooms),
                ['4'] = new MenuItem("Brisanje sobe", DeleteRoom),
                ['5'] = new MenuItem("Povratak na prethodni izbornik", () => MenuAction.Exit)
            }, "SOBE");

            return menu.Run();
        }

        static MenuAction ShowAllRooms()
        {
            Console.Clear();

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
            var category = Menu.ReadRequiredEnum<RoomCategory>("Unesite kategoriju sobe", "Molimo unesite valjanu kategoriju!");
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
            Console.Clear();

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
            if (string.IsNullOrEmpty(targetGuid))
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

        static MenuAction ShowPersonnelMenu()
        {
            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Prikaz svih smjena", ShowAllShifts),
                ['2'] = new("Kreiranje nove smjene", AddNewShift),
                ['3'] = new("Dodavanje zaposlenika", AddNewEmployee),
                ['4'] = new("Povratak na prethodni izbornik", () => MenuAction.Exit)
            }, "OSOBLJE");

            return menu.Run();
        }

        static MenuAction ShowAllShifts()
        {
            Console.Clear();

            if (Shift.All.Count == 0)
            {
                Console.WriteLine("Nema smjena.");
                return MenuAction.PauseAndStay;
            }

            foreach (var shift in Shift.All)
                Console.WriteLine(shift);

            return MenuAction.PauseAndStay;
        }

        static MenuAction AddNewShift()
        {
            var receptionists = Employee.All.Where(e => e.Position == EmployeePosition.Receptionist).ToList();
            var stewards = Employee.All.Where(e => e.Position == EmployeePosition.Steward).ToList();
            var cooks = Employee.All.Where(e => e.Position == EmployeePosition.Cook).ToList();

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(receptionists.Count, "Broj recepcionista/ica");
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cooks.Count, "Broj kuhara/ica");
            ArgumentOutOfRangeException.ThrowIfLessThan(stewards.Count, 2, "Broj sobara/ica");

            Console.Clear();

            var receptionist = Menu.SelectFromList(receptionists, "Odaberite recepcionista/icu");
            var stewardA = Menu.SelectFromList(stewards, "Odaberite sobara/icu");
            stewards.Remove(stewardA);
            var stewardB = Menu.SelectFromList(stewards, "Odaberite drugog/u sobara/icu");
            var cook = Menu.SelectFromList(cooks, "Odaberite kuhara/icu");
            var shiftDate = Menu.ReadRequiredMatching<DateOnly>("Unesite datum smjene", 
                dt => dt >= DateOnly.FromDateTime(DateTime.Today), "Unesite datum",
                "Datum mora biti u budućnosti!");
            var shiftType = Menu.IsPromptPositive("Je li smjena popodnevna?");

            if (Menu.IsPromptPositive("Želite li dodati smjenu?"))
            {
                _ = new Shift
                {
                    EmployeeGuids =
                    {
                        receptionist.Guid,
                        stewardA.Guid,
                        stewardB.Guid,
                        cook.Guid
                    },
                    ShiftDate = shiftDate,
                    IsAfternoon = shiftType
                };
                Console.WriteLine("Smjena uspješno dodana.");
                return MenuAction.PauseAndStay;
            }

            return MenuAction.Stay;
        }

        static MenuAction AddNewEmployee()
        {
            var name = Menu.ReadRequiredString("Unesite ime", "Molimo unesite ime!");
            var surname = Menu.ReadRequiredString("Unesite prezime", "Molimo unesite prezime!");
            var birthYear = Menu.ReadRequired<int>("Unesite godinu rođenja", "Molimo unesite godinu!");
            var gender = Menu.ReadRequiredEnum<Gender>("Unesite spol", "Molimo unesite spol!");
            var position = Menu.ReadRequiredEnum<EmployeePosition>("Unesite poziciju", "Molimo unesite poziciju!");

            if (Menu.IsPromptPositive("Želite li dodati zaposlenika?"))
            {
                _ = new Employee
                {
                    Name = name,
                    Surname = surname,
                    BirthYear = birthYear,
                    Gender = gender,
                    Position = position
                };
                Console.WriteLine("Zaposlenik uspješno dodan.");
                return MenuAction.PauseAndStay;
            }
            else
                return MenuAction.Stay;
        }

        static void ReplaceEntity<T>(List<T> list, T item, Func<T, T> modification)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(modification);

            var index = list.IndexOf(item);
            item = modification(item);
            list[index] = item;
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

            _ = new Reservation(IvicaH, RoomA, new(2026, 12, 10), new(2026, 12, 17), false);
            _ = new Reservation(FranjoF, RoomC, new(2026, 12, 5), new(2026, 12, 8), false);
            _ = new Reservation(MateM, RoomE, new(2026, 12, 12), new(2026, 12, 15), false);
            _ = new Reservation(FranjoF, RoomB, new(2026, 12, 3), new(2026, 12, 5), false);
            _ = new Reservation(IvicaH, RoomD, new(2026, 12, 1), new(2026, 12, 5), false);

            var Receptionist = new Employee
            {
                Name = "Ante",
                Surname = "Antić",
                BirthYear = 1984,
                Position = EmployeePosition.Receptionist,
                Gender = Gender.Male
            };
            var StewardA = new Employee
            {
                Name = "Ana",
                Surname = "Anić",
                BirthYear = 1979,
                Position = EmployeePosition.Steward,
                Gender = Gender.Female
            };
            var StewardB = new Employee
            {
                Name = "Matea",
                Surname = "Matić",
                BirthYear = 1988,
                Position = EmployeePosition.Steward,
                Gender = Gender.Female
            };
            var Cook = new Employee
            {
                Name = "Mladen",
                Surname = "Grubišić",
                BirthYear = 1975,
                Position = EmployeePosition.Cook,
                Gender = Gender.Male
            };

            var Shift = new Shift
            {
                IsAfternoon = true,
                ShiftDate = new DateOnly(2026, 12, 10),
                EmployeeGuids = [
                    Cook.Guid,
                    StewardB.Guid,
                    StewardA.Guid,
                    Receptionist.Guid
                ]
            };
        }
    }
}

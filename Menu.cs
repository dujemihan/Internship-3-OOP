namespace Internship_3_OOP
{
    internal sealed record MenuContext
    {
        public Guest? Guest { get; init; }
        public Reservation? Reservation { get; init; }
        public Room? Room { get; init; }
    }

    internal sealed record MenuItem
    {
        public string Name { get; }
        public Func<MenuContext?, MenuAction> Action { get; }

        internal MenuItem(string name, Func<MenuAction> action)
        {
            Name = name;
            Action = _ => action();
        }

        internal MenuItem(string name, Func<MenuContext?, MenuAction> action)
        {
            Name = name;
            Action = action;
        }
    };

    internal sealed class Menu(Dictionary<char, MenuItem> items, string? header = null)
    {
        private readonly Dictionary<char, MenuItem> items = items;
        public string? Header { get; init; } = header;

        public MenuAction Run(MenuContext? context = null)
        {
            while (true)
            {
                switch (Show(context))
                {
                    case MenuAction.PauseAndStay:
                        WaitForKey();
                        break;
                    case MenuAction.Stay:
                        break;
                    default:
                        return MenuAction.Stay;
                }
            }
        }

        public MenuAction Show(MenuContext? context = null)
        {
            Console.Clear();

            if (Header is not null)
                Console.WriteLine($"{Header}\n");

            foreach (var (k, v) in items)
                Console.WriteLine($"{k} - {v.Name}");

            Console.WriteLine();
            while (true)
            {
                Console.Write("Odaberite opciju: ");
                var choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                choice = char.ToLower(choice);
                if (items.TryGetValue(choice, out var item))
                    return item.Action(context);
                else
                    Console.WriteLine("Molimo odaberite valjanu opciju!");
            }
        }

        public static T SelectFromList<T>(IList<T> list,
            string prompt = "Unesite odabir",
            string invalidMsg = "Molimo unesite valjan odabir!") where T : class
        {
            for (int i = 0; i < list.Count; i++)
                Console.WriteLine($"{i + 1}. {list[i]}");

            T? element;
            do
            {
                element = list.ElementAtOrDefault(ReadRequired<int>(prompt, invalidMsg) - 1);
            } while (element == default);

            return element;
        }

        public static void WaitForKey()
        {
            Console.WriteLine("Pritisnite bilo koju tipku za nastavak...");
            Console.ReadKey();
        }

        public static bool IsPromptPositive(in string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (Y/N) ");
                var choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (choice == 'y' || choice == 'Y')
                    return true;
                else if (choice == 'n' || choice == 'N')
                    return false;
            }
        }

        public static string ReadRequiredString(string prompt, string? invalidMsg = null)
        {
            while (true)
            {
                var str = ReadOptionalString(prompt);

                if (str is null)
                {
                    if (invalidMsg is not null)
                        Console.WriteLine(invalidMsg);
                }
                else
                    return str;
            }
        }

        public static string? ReadOptionalString(string prompt, string? fallback = null)
        {
            Console.Write($"{prompt}: ");
            var str = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(str))
                return fallback;
            else
                return str;
        }

        public static T ReadRequired<T>(string prompt, string? invalidMsg = null) where T : IParsable<T>
        {
            T? ret;

            while (!T.TryParse(ReadRequiredString(prompt, invalidMsg), null, out ret))
                Console.WriteLine(invalidMsg);

            return ret;
        }

        public static T? ReadOptional<T>(string prompt, T? fallback = default) where T : IParsable<T>
        {
            return T.TryParse(ReadOptionalString(prompt), null, out var ret) ? ret : fallback;
        }

        public static T ReadRequiredEnum<T>(string prompt, string? invalidMsg = null) where T : struct, Enum
        {
            T ret;

            Console.WriteLine($"Moguće vrijednosti: {string.Join(',', Enum.GetValues<T>())}");
            while (!Enum.TryParse(ReadRequiredString(prompt, invalidMsg), true, out ret)) ;
            return ret;
        }

        public static T ReadRequiredMatching<T>(string prompt, Predicate<T> condition, string? invalidMsg = null,
            string? unmatchingMsg = null) where T : IParsable<T>
        {
            T? ret;

            while (true)
            {
                ret = ReadRequired<T>(prompt, invalidMsg);
                if (!condition(ret))
                    Console.WriteLine(unmatchingMsg);
                else
                    break;
            }

            return ret;
        }
    }
}
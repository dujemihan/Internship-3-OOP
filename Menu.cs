namespace Internship_3_OOP
{
    internal sealed record MenuItem(string Name, Func<MenuAction> Action);

    internal sealed class Menu(Dictionary<char, MenuItem> items)
    {
        private readonly Dictionary<char, MenuItem> items = items;

        public MenuAction Show()
        {
            Console.Clear();
            foreach (var (k, v) in items)
                Console.WriteLine($"{k} - {v.Name}");

            while (true)
            {
                Console.Write("Odaberite opciju: ");
                var choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                choice = char.ToLower(choice);
                if (items.TryGetValue(choice, out var item))
                    return item.Action();
                else
                    Console.WriteLine("Molimo odaberite valjanu opciju!");
            }
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

        public static string ReadRequiredString(string prompt, string? invalidMsg)
        {
            while (true)
            {
                var str = ReadOptionalString(prompt, "");

                if (str == "")
                {
                    if (invalidMsg is not null)
                        Console.WriteLine(invalidMsg);
                }
                else
                    return str;
            }
        }

        public static int ReadRequiredInt(string prompt, string? invalidMsg)
        {
            int ret;

            while (!int.TryParse(ReadRequiredString(prompt, invalidMsg), out ret))
                Console.WriteLine(invalidMsg);

            return ret;
        }

        static string ReadOptionalString(string prompt, string fallback)
        {
            Console.Write($"{prompt}: ");
            var str = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(str))
                return fallback;
            else
                return str;
        }
    }
}
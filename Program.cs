using System.Text;

namespace Internship_3_OOP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            var menu = new Menu(new Dictionary<char, MenuItem>
            {
                ['1'] = new("Gosti", ShowGuestsMenu),
                ['0'] = new("Izlaz iz programa", () => MenuAction.Exit)
            });

            while (menu.Show() == MenuAction.Stay) ;
        }

        static MenuAction ShowGuestsMenu()
        {
            return MenuAction.Stay;
        }
    }
}

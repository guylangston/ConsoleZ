using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleZ.Drawing;

namespace ConsoleZ.Samples
{
    public static class SampleDocuments
    {

        public static void MarkDownBasics(IConsole console)
        {
            // https://guides.github.com/features/mastering-markdown/
            console.WriteLine("# Header");
            console.WriteLine("");
            console.WriteLine(
                "It's very easy to make some words **bold** and other words *italic* with Markdown. You can even [link to Google!](http://google.com) ");

            console.WriteLine("List:");
            console.WriteLine("- Item One");
            console.WriteLine("- Item Two");
            console.WriteLine("- Item Three");

            console.WriteLine(@"But I have to admit, tasks lists are my favorite:

- [x] This is a complete item
- [ ] This is an incomplete item");


        }


        public static void ColourPalette(IConsole console)
        {
            // https://guides.github.com/features/mastering-markdown/
            console.WriteLine("# Colours");
            console.WriteLine("");

            foreach (var color in ConsoleHelper.ToConsole)
            {
                console.WriteLine($"{color.Key.Name,12} => ^{color.Key.Name};XXXX...^; reverted.");
            }
            

        }

    }
}

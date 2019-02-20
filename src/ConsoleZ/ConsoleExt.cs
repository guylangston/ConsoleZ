using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleZ
{
    public static class ConsoleExt
    {
        public static int WriteLine(this IConsole cons) => cons.WriteLine(null);

        public static int WriteObj(this IConsole cons, object obj) => cons.WriteFormatted($"{obj}");

        public static int WriteException(this IConsole cons, Exception ex) => cons.WriteFormatted($"{ex}");
    }

    public static class DefaultConsole
    {
        public static IConsole Console { get; set; } = StdConsole.Singleton;
    }
}

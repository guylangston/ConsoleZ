using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConsoleZ
{
    public class VirtualConsole : ConsoleBase
    {
        public VirtualConsole(string handle, int width, int height) : base(handle.ToString(), width, height)
        {
            
        }

        
        public override void LineChanged(int index, string line, bool updated)
        {
            // Nothing
            Console.WriteLine(line);
        }

        public IReadOnlyList<string> GetTextLines() => base.lines;
    }

    public interface IVirtualConsoleRepository
    {
        bool TryGetConsole(string handle, out VirtualConsole cons);
        IConsole AddConsole(VirtualConsole cons);
    }

    /// <summary>
    /// Do not use in producton.
    /// </summary>
    /// <typeparam name="T">Handle Type</typeparam>
    public sealed class StaticVirtualConsoleRepository : IVirtualConsoleRepository
    {
        private readonly ConcurrentDictionary<string, VirtualConsole> consoleList;

        private StaticVirtualConsoleRepository()
        {
            consoleList = new ConcurrentDictionary<string, VirtualConsole>();
        }

        private static readonly object locker = new object();


        private static StaticVirtualConsoleRepository inst = null;
        public static StaticVirtualConsoleRepository Singleton
        {
            get
            {
                if (inst != null) return inst;
                lock (locker)
                {
                    if (inst != null) return inst;
                    inst = new StaticVirtualConsoleRepository();
                    return inst;
                }
            }
        }
        

        public bool TryGetConsole(string handle, out VirtualConsole cons) => consoleList.TryGetValue(handle, out cons);

        public IConsole AddConsole(VirtualConsole cons)
        {
            consoleList[cons.Handle] = cons;
            return cons;
        } 
    }
}
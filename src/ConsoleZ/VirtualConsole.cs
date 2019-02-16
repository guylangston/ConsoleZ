using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConsoleZ
{
    public class VirtualConsole : ConsoleBase
    {
        public VirtualConsole(string handle, int width, int height) : base(handle, width, height)
        {
            
        }

        
        public override void LineChanged(int index, string line, bool updated)
        {
            // Nothing
            Console.WriteLine(Renderer.RenderLine(this, index, line));
        }

        public IReadOnlyList<string> GetTextLines() => base.lines;
    }

    public interface IVirtualConsoleRepository
    {
        bool TryGetConsole(string handle, out VirtualConsole cons);
        IConsoleWithProps AddConsole(VirtualConsole cons);
    }

    /// <summary>
    /// Do not use in production.
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
        private static volatile StaticVirtualConsoleRepository instance = null;
        public static StaticVirtualConsoleRepository Singleton
        {
            get
            {
                if (instance != null) return instance;
                lock (locker)
                {
                    if (instance != null) return instance;
                    return instance = new StaticVirtualConsoleRepository();
                }
            }
        }
        

        public bool TryGetConsole(string handle, out VirtualConsole cons) => consoleList.TryGetValue(handle, out cons);

        public IConsoleWithProps AddConsole(VirtualConsole cons)
        {
            consoleList[cons.Handle] = cons;
            return cons;
        } 
    }
}
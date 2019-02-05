using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConsoleZ
{
    public class VirtualConsole<T> : ConsoleBase
    {
        public VirtualConsole(T handle, int width, int height) : base(handle.ToString(), width, height)
        {
            this.Handle = handle;
        }

        public new T Handle { get; }


        public override void LineChanged(int index, string line, bool updated)
        {
            // Nothing
            Console.WriteLine(line);
        }

        public IReadOnlyList<string> GetTextLines() => base.lines;
    }

    public interface IVirtualConsoleRepository<T>
    {
        bool TryGetConsole(T handle, out VirtualConsole<T> cons);
        IConsole  AddConsole(VirtualConsole<T> cons);
    }

    /// <summary>
    /// Do not use in producton.
    /// </summary>
    /// <typeparam name="T">Handle Type</typeparam>
    public sealed class StaticVirtualConsoleRepository<T> : IVirtualConsoleRepository<T>
    {
        private readonly ConcurrentDictionary<T, VirtualConsole<T>> consoleList;

        private StaticVirtualConsoleRepository()
        {
            consoleList = new ConcurrentDictionary<T, VirtualConsole<T>>();
        }

        private static readonly object locker = new object();


        private static StaticVirtualConsoleRepository<T> inst = null;
        public static StaticVirtualConsoleRepository<T> Singleton
        {
            get
            {
                if (inst != null) return inst;
                lock (locker)
                {
                    if (inst != null) return inst;
                    inst = new StaticVirtualConsoleRepository<T>();
                    return inst;
                }
            }
        }
        

        public bool TryGetConsole(T handle, out VirtualConsole<T> cons) => consoleList.TryGetValue(handle, out cons);

        public IConsole AddConsole(VirtualConsole<T> cons)
        {
            consoleList[cons.Handle] = cons;
            return cons;
        } 
    }
}
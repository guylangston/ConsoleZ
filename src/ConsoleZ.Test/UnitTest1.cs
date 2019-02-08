using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xunit;

namespace ConsoleZ.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
        }
    }
    
    

    public struct Token<T>
    {
        public int Start { get; set; }
        public int End { get; set; }
        public T Ext { get; set; }
        
    }

    public abstract class TokenParser<T>
    {
        private List<Token<T>> tokens = new List<Token<T>>();

        public void Scan(Span<char> text)
        {
            var c = 0; // cursor
            while (c < text.Length)
            {
                if (IsStart(text, c, text[c]))
                {
                    var e = c + 1;
                    while (e < text.Length)
                    {
                        if (IsEnd(text, c, e, text[e]))
                        {
                            tokens.Add(CreateToken(c, e));
                        }
                    }
                }
            }
        }

        protected abstract Token<T> CreateToken(int start, int end);

        protected abstract bool IsEnd(Span<char> text, int start, int end, char endC);

        protected abstract bool IsStart(Span<char> text, int start, char c);
    }
}
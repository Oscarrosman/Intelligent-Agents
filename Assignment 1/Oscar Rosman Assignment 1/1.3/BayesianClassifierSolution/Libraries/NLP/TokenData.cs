using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP
{
    public class TokenData
    {
        public Token Token { get; set; }
        public int Count { get; set; }
        public double Class0Count { get; set; }    // Not relevant in Assignment 1.1, but used in Assignment 1.2 and 1.3
        public double Class1Count { get; set; }    // Not relevant in Assignment 1.1, but used in Assignment 1.2 and 1.3

        public TokenData(Token token, int label)
        {
            Token = token;
            Count = 1;
            Class0Count = label == 0 ? 1 : 0;
            Class1Count = label == 1 ? 1 : 0;
        }

        public void AddInstance(int label)
        {
            Count++;
            if (label == 0) { Class0Count += 1; }
            else { Class1Count += 1; }
        }
    }
}

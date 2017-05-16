using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL1
{
    class Process
    {
        private int index;
        private string symbolStack;
        private string inputString;
        private string production;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public string SymbolStack
        {
            get { return symbolStack; }
            set { symbolStack = value; }
        }

        public string InputString
        {
            get { return inputString; }
            set { inputString = value; }
        }

        public string Production
        {
            get { return production; }
            set { production = value; }
        }
    }
}

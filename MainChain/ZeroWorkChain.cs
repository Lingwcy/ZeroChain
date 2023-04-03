using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero.NetChain;

namespace Zero.MainChain
{
    public class ZeroWorkChain
    {
        public IList<Block> Chain { get; set; }

        public ZeroWorkChain()
        {
            Chain = new List<Block>();
        }

        public void ShowBlockChian()
        {
            foreach (Block block in Chain)
            {
                block.PrintBlock();
            }
        }

    }
}

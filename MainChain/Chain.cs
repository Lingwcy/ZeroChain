using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Zero.NetChain;
using Zero.TOKEN;

namespace Zero.MainChain
{
    public class Chain
    {
        public IList<Block> _Chain { get; set; }
        public virtual Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now, "创世块", "0", new ConcurrentQueue<Transaction>());
        }
        public virtual Block GetLatestBlock()
        {
            return _Chain[_Chain.Count - 1];
        }
        public virtual bool IsValid()
        {
            for (int i = 1; i < _Chain.Count; i++)
            {
                Block currentBlock = _Chain[i];
                Block previousBlock = _Chain[i - 1];
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    Console.WriteLine($"错误的区块{currentBlock.Index}\n" +
                        $"{currentBlock.Hash} != {currentBlock.CalculateHash()}");
                    currentBlock.PrintBlock();
                    return false;
                }
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    Console.WriteLine($"错误的区块{currentBlock.Index}\n" +
                        $"{currentBlock.Hash} != {previousBlock.Hash}");
                    currentBlock.PrintBlock();
                    return false;
                }
            }
            return true;
        }

        public virtual void ShowBlockChain()
        {
            foreach (Block block in _Chain)
            {
                block.PrintBlock();
            }
        }

        public virtual void ShowBlockChainInfo()
        {
            Console.WriteLine($"区块链信息: \n" +
                $"区块高度:{_Chain.Count}\n");
        }
    }
}

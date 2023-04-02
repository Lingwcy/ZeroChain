
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;
using System.Transactions;
using Zero.TOKEN;

namespace Zero.NetChain
{
    public class Blockchain<T> where T : TokenBase
    {
        private IList<Block> _chain;
        private T _token;//主治代币
        public TokenBook TokenBook { get; set; }
        public int ProofOfWorkDifficulty { get; set; } = 2;
        public decimal MiningReward { get; set; } = 10;
        public ConcurrentQueue<Transaction> PendingTransactions { get; set; }

        public int MAXTransactionsCount { get; init; } = 5;//每个区块最大交易笔数

        public int FullBlockChainHight { get; set; }//被验证且填满交易笔数的区块
        public int AvaibleBlockChainHight { get; set; }//已经被验证的区块

        public Blockchain()
        {
            _chain = new List<Block> { CreateGenesisBlock() };
            PendingTransactions = new ConcurrentQueue<Transaction>();
            TokenBook = new TokenBook();
            _token = (T)Activator.CreateInstance(typeof(T), TokenBook.TokenList.Count, "Zero", new Decimal(1), new decimal(10000), new  decimal(0));
            TokenBook.TokenList.Add(_token);
            PrintChianInfo();
        }

        private Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now, "创世块", "0", new List<Transaction>());
        }

        public Block GetLatestBlock()
        {
            return _chain[_chain.Count-1];
        }


        public bool IsValid()
        {
            for (int i = 1; i < _chain.Count; i++)
            {
                Block currentBlock = _chain[i];
                Block previousBlock = _chain[i - 1];
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }


        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if(GetBalance(transaction.FromAddress)<transaction.Amount)
            {
                Console.WriteLine($"{transaction.FromAddress} 余额不足");
                return;
            }
            Console.WriteLine((GetBalance(transaction.FromAddress)));
            PendingTransactions.Enqueue(transaction);
        }

        public void MineNewBlock (string miningRewardAddress)
        {
            var block = new Block(_chain.Count,DateTime.UtcNow,"data","priHash", new List<Transaction>());
            block.MineBlock(ProofOfWorkDifficulty);
            block.PreviousHash = GetLatestBlock().Hash;
            if(_token.CurrentSupply+ MiningReward > _token.TotalSupply)
            {
                MiningReward = 0;
            }
            block.Transactions.Add(new Transaction("NEW ZERO", miningRewardAddress, MiningReward));
            _token.CurrentSupply += MiningReward;
            _chain.Add(block);
            AvaibleBlockChainHight++;
        }
        public void MinePendingTransactions()
        {
            while (true)
            {
                if (PendingTransactions.Count != 0)
                {
                    if (_chain[FullBlockChainHight].Transactions.Count < MAXTransactionsCount)
                    {
                        PendingTransactions.TryDequeue(out var dealing);
                        _chain[FullBlockChainHight].Transactions.Add(dealing);
                        Console.WriteLine("交易成功!");
                    }
                    else
                    {
                        if (AvaibleBlockChainHight > FullBlockChainHight) { FullBlockChainHight++; }
                        else { Console.WriteLine("没有新的可用区块"); }
                    }
                }
            }
        }
        public decimal GetBalance(string address)
        {
            decimal balance = 0;

            foreach (var block in _chain)
            {
                foreach (var transaction in block.Transactions)
                {
                    if (transaction.FromAddress == address)
                    {
                        balance -= transaction.Amount;
                    }

                    if (transaction.ToAddress == address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            foreach (var transaction in PendingTransactions)
            {
                if (transaction.FromAddress == address)
                {
                    balance -= transaction.Amount;
                }
                if (transaction.ToAddress == address)
                {
                    balance += transaction.Amount;
                }
            }

            return balance;
        }

        //测试方法
        public void AttackChainBlock(int i)
        {
            _chain[i].Hash = "000xxxAttacted";
        }

        public void ShowBlockChian()
        {
            foreach (Block block in _chain)
            {
                // 对每一个区块执行相应的操作
                // 比如打印区块的数据等
                Console.WriteLine("区块代号: " + block.Index);
                Console.WriteLine("生成时间: " + block.Timestamp);
                Console.WriteLine("块哈希值: " + block.Hash);
                Console.WriteLine("前块哈希值: " + block.PreviousHash);
                Console.WriteLine("区块数据: " + block.Data);
                Console.WriteLine("区块交易: ");
                int i = 1;
                foreach(var tr in block.Transactions)
                {
                    Console.WriteLine($"{i}.时间:{tr.Time} 发送地址:{tr.FromAddress} 接收地址:{tr.ToAddress}");
                    i++;
                }
                Console.WriteLine("------------------------------------");
            }
        }

        public void PrintChianInfo()
        {
            Console.WriteLine($"区块链信息: \n" +
                $"区块高度:{_chain.Count}\n" +
                $"主治代币:{_token.Name}\n" +
                $"最大供应量:{_token.TotalSupply}\n" +
                $"目前供应量:{_token.CurrentSupply}\n" +
                $"价格:{_token.Value}\n" +
                $"链中代币数量:{TokenBook.TokenList.Count}");
        }
    }
}

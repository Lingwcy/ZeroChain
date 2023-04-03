
using System.Collections.Concurrent;
using Zero.MainChain;
using Zero.NetChain.Dealing;
using Zero.TOKEN;

namespace Zero.NetChain
{
    public class ZeroMineChain<T> where T : TokenBase
    {
        private IList<Block> Chain {get;set;} //矿工链
        private ZeroWorkChain WorkChain { get;set;} //不可篡改链
        private T _token;//主治代币
        public TokenBook TokenBook { get; set; }
        public int ProofOfWorkDifficulty { get; set; } = 0;
        public decimal MiningReward { get; set; } = 10;
        public ConcurrentQueue<Transaction> PendingTransactions { get; set; }
        public int MAXTransactionsCount { get; init; } = 5;//每个区块最大交易笔数
        public int FullBlockChainHight { get; set; }//被验证且填满交易笔数的区块
        public int AvaibleBlockChainHight { get; set; }//已经被验证的区块

        public ZeroMineChain()
        {
            Chain = new List<Block> { CreateGenesisBlock() };
            PendingTransactions = new ConcurrentQueue<Transaction>();
            TokenBook = new TokenBook();
            _token = (T)Activator.CreateInstance(typeof(T), TokenBook.TokenList.Count, "Zero", new Decimal(1), new decimal(10000), new  decimal(0));
            TokenBook.TokenList.Add(_token);
            WorkChain= new ZeroWorkChain();
            ShowBlockChainInfo();
        }

        private Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now, "创世块", "0", new ConcurrentQueue<Transaction>());
        }

        public Block GetLatestBlock()
        {
            return Chain[Chain.Count-1];
        }



        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (GetBalance(transaction.FromAddress) < transaction.Amount)
            {
                Console.WriteLine($"{transaction.FromAddress} 余额不足");
                return;
            }
            PendingTransactions.Enqueue(transaction);
        }

        public void MineNewBlock (string miningRewardAddress)
        {
            var block = new Block(Chain.Count,DateTime.UtcNow,"data","priHash", new ConcurrentQueue<Transaction>());
            block.MineBlock(ProofOfWorkDifficulty);
            block.PreviousHash = GetLatestBlock().Hash;
            if(_token.CurrentSupply+ MiningReward > _token.TotalSupply)
            {
                MiningReward = 0;
            }
            block.Transactions.Enqueue(new Transaction("NEW ZERO", miningRewardAddress, MiningReward));
            block.Hash= block.CalculateHash();
            _token.CurrentSupply += MiningReward;
            Chain.Add(block);
            AvaibleBlockChainHight++;
        }
        public void MinePendingTransactions()
        {
            while (true)
            {
                if (PendingTransactions.Count != 0)
                {
                    if (Chain[FullBlockChainHight].Transactions.Count < MAXTransactionsCount)
                    {
                        PendingTransactions.TryDequeue(out var dealing);
                        Chain[FullBlockChainHight].Transactions.Enqueue(dealing);
                        Chain[FullBlockChainHight].MineBlock(ProofOfWorkDifficulty);
                        Console.WriteLine("交易成功!");
                    }
                    else
                    {
                        if (AvaibleBlockChainHight > FullBlockChainHight)
                        {
                            WorkChain.Chain.Add(Chain[FullBlockChainHight]);
                            FullBlockChainHight++; 
                        }
                        else { Console.WriteLine("没有新的可用区块"); }
                    }
                }
            }
        }
        public decimal GetBalance(string address)
        {
            decimal balance = 0;

            foreach (var block in Chain)
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

        public void ShowBlockChainInfo()
        {
            Console.WriteLine($"区块链信息: \n" +
                $"区块高度:{Chain.Count}\n" +
                $"主治代币:{_token.Name}\n" +
                $"最大供应量:{_token.TotalSupply}\n" +
                $"目前供应量:{_token.CurrentSupply}\n" +
                $"价格:{_token.Value}\n" +
                $"链中代币数量:{TokenBook.TokenList.Count}");
        }

        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];
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

        public void ShowBlockChain()
        {
            foreach (Block block in Chain)
            {
                block.PrintBlock();
            }
        }
    }
}

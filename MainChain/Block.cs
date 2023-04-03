using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Zero.NetChain.Dealing;

namespace Zero.NetChain
{
    public class Block
    {
        private int v1;
        private DateTime now;
        private string v2;
        private string v3;
        private ConcurrentQueue<System.Transactions.Transaction> transactions;

        public int Index { get; set; }  // 区块编号
        public DateTime Timestamp { get; set; }  // 时间戳
        public string Hash { get; set; }  // 当前区块哈希值
        public string PreviousHash { get; set; }  // 前一个区块哈希值
        public string Data { get; set; }  // 区块体数据
        public int Nonce { get; set; } //随机值
        public ConcurrentQueue<Transaction> Transactions { get; set; }

        public Block(int index, DateTime timestamp, string data, string previousHash, ConcurrentQueue<Transaction> transactions)
        {
            Index = index;
            Timestamp = timestamp;
            Data = data;
            PreviousHash = previousHash;
            Hash = CalculateHash();
            Nonce = 0;
            Transactions = transactions;
        }

        public Block(int v1, DateTime now, string v2, string v3, ConcurrentQueue<System.Transactions.Transaction> transactions)
        {
            this.v1 = v1;
            this.now = now;
            this.v2 = v2;
            this.v3 = v3;
            this.transactions = transactions;
        }

        public string CalculateHash()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes($"{Index}-{Timestamp}-{Data}-{PreviousHash}-{Nonce}-{Transactions}");
                byte[] outputBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(outputBytes);
            }
        }

        public void MineBlock(int difficulty)
        {
            string target = new string('0', difficulty);
            while (Hash.Substring(0, difficulty) != target)
            {
                Nonce++;
                Hash = CalculateHash();
            }
            Console.WriteLine($"区块挖出: {Hash}");
        }

        public void PrintBlock()
        {
            Console.WriteLine("区块代号: " + Index);
            Console.WriteLine("生成时间: " + Timestamp);
            Console.WriteLine("块哈希值: " + Hash);
            Console.WriteLine("前块哈希值: " + PreviousHash);
            Console.WriteLine("区块数据: " + Data);
            Console.WriteLine("区块难度: " + Nonce);
            Console.WriteLine("区块交易: ");
            int i = 1;
            foreach (var tr in Transactions)
            {
                Console.WriteLine($"{i}.时间:{tr.Time} 发送地址:{tr.FromAddress} 接收地址:{tr.ToAddress}");
                i++;
            }
            Console.WriteLine("///////////////////////////////////////////");
        }
    }

}

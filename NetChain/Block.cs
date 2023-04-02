using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Zero.NetChain
{
    public class Block
    {
        public int Index { get; set; }  // 区块编号
        public DateTime Timestamp { get; set; }  // 时间戳
        public string Hash { get; set; }  // 当前区块哈希值
        public string PreviousHash { get; set; }  // 前一个区块哈希值
        public string Data { get; set; }  // 区块体数据
        public int Nonce { get; set; } //随机值
        public IList<Transaction> Transactions { get; set; }

        public Block(int index, DateTime timestamp, string data, string previousHash, IList<Transaction> transactions)
        {
            Index = index;
            Timestamp = timestamp;
            Data = data;
            PreviousHash = previousHash;
            Hash = CalculateHash();
            Nonce = 0;
            Transactions = transactions;
        }

        public string CalculateHash()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes($"{Index}-{Timestamp}-{Data}-{PreviousHash}-{Nonce}");
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
    }
}

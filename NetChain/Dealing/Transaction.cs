using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zero.TOKEN;

namespace Zero.NetChain.Dealing
{
    public class Transaction
    {
        public Transaction(string fromAddress, string toAddress, decimal amount)
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Time = DateTime.Now;
            Amount = amount;
        }

        public string FromAddress { get; set; }
        public string ToAddress { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }

    }
}

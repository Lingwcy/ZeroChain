
using Zero.NetChain;

namespace Zero
{
    using System;
    using Zero.NetChain.Dealing;
    using Zero.TOKEN;

    class Program
    {
        static void Main(string[] args)
        {
            DealingTest();
        }

        public static void DealingTest()
        {
            Blockchain<ZERO> chain = new Blockchain<ZERO>();
            for (int i = 0; i < 1; i++)
            {
                chain.MineNewBlock("0x123456789");
            }
            Task.Factory.StartNew(chain.MinePendingTransactions);
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Transaction t = new Transaction("0x123456789", "test", 1.5M);
                    chain.AddTransaction(t);
                }
            });
            Task.Factory.StartNew(() =>
            {
                for (int i = 1; i < 1; i++)
                {
                    Transaction t = new Transaction("test", "0x123456789", 1.5M);
                    chain.AddTransaction(t);
                }
            });


            chain.PrintChianInfo();

            Console.ReadLine();
            chain.ShowBlockChian();
            Console.WriteLine("余额：" + chain.GetBalance("0x123456789"));
            Console.WriteLine("余额：" + chain.GetBalance("test"));
            Console.WriteLine(chain.IsValid());
        }
    }
}
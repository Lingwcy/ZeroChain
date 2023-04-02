using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Zero.TOKEN
{
    public abstract class TokenBase
    { 
        public int Id { get; init; }
        public string Name { get; init; }
        public decimal Value { get; set; }
        public decimal TotalSupply { get; init; }
        public decimal CurrentSupply
        {
            get { return currentSupply; }
            set
            {
                if (TotalSupply <= CurrentSupply)
                {
                    return;
                }
                currentSupply = value;
            }
        }

        private decimal currentSupply;

        public TokenBase(int id,string name, decimal value, decimal totalSupply, decimal currentSupply)
        {
            Id = id;
            Name = name;
            Value = value;
            TotalSupply = totalSupply;
            this.currentSupply = currentSupply;
        }
    }
}

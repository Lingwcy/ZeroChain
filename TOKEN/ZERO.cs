using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Zero.TOKEN
{
    public class ZERO : TokenBase
    {
        public ZERO(int id,string name, decimal value, decimal totalSupply, decimal currentSupply) : base(id,name, value, totalSupply, currentSupply)
        {
        }

    }
}

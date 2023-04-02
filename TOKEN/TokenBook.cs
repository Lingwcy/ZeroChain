using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zero.TOKEN
{
    public class TokenBook
    {
        public IList<TokenBase> TokenList { get; set; }
        public TokenBase TokenBaseToken { get; init;} 
        public TokenBook()
        {
            TokenList = new List<TokenBase>();
        }
    }
}

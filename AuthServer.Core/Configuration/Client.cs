using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
    public class Client
    {
        public string Id{ get; set; }
        public string Secret{ get; set; }

        // audience üzerinde hangi api erişim sağlar onu göstermektedir.
        public List<String> Audiences { get; set; }
    }
}

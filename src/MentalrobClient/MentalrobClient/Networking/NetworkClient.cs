using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MentalrobClient.Networking
{
    class NetworkClient
    {
        public static Telepathy.Client Current;
        
        public NetworkClient()
        {
            if(Current == null)
            {
                Current = new Telepathy.Client(1024*1024);
            }
        }
    }
}

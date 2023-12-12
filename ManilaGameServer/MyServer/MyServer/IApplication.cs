using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public interface IApplication
    {
        void Disconnect(ClientPeer client);

        void Receive(ClientPeer client, NetMsg msg);
    }
}

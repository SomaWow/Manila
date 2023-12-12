using MyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Logic
{
    public interface IHandler
    {
        void Disconnect(ClientPeer client);
        void Receive(ClientPeer client, int subCode, object value);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    class ClientPeerPool
    {
        private Queue   <ClientPeer> clientPeerQueue;

        public ClientPeerPool(int maxClient)
        {
            clientPeerQueue = new Queue<ClientPeer>(maxClient);
        }
        public void Enqueue(ClientPeer client)
        {
            clientPeerQueue.Enqueue(client);
        }
        public ClientPeer Dequeue()
        {
            return clientPeerQueue.Dequeue();
        }
    }
}

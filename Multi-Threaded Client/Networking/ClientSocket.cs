using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Information;

namespace Multi_Threaded_Client.Networking
{
    class ClientSocket
    {
        public Socket _ClientSocket;

        public ClientSocket()
        {
            _ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string IP)
        {
            _ClientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP),5000));
        }




    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Information;

namespace Multi_Threaded_Server.Networking
{
    class ServerSocket
    {
        private Socket _ServerSocket;
        public List<Socket> _ClientSockets;
        public Thread _AcceptThread { set; get; }
        
        
        public ServerSocket()
        {
            _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _ClientSockets = new List<Socket>(6);
        }

        public void Bind()
        {
            _ServerSocket.Bind(new IPEndPoint(IPAddress.Any,5000));
        }

        public void Listen()
        {
            _ServerSocket.Listen(20);
        }
        
        public void AcceptThread()
        {
            for (int i=0;i<6;i++ )
            {           
                _ClientSockets.Add(_ServerSocket.Accept());
                SendID(i);//寄送ID給每個連接的client
            }
        }

        public void SendID(int i)//寄送ID
        {
            _ClientSockets[i].Send(BitConverter.GetBytes(i));
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Multi_Threaded_Server.Networking;
using Information;

namespace Multi_Threaded_Server
{
    public partial class ServerForm : Form
    {
        ServerSocket _ServerSocket;
        PersonalInfo AllInfo;
        Thread _ReceiveInfoThread;
        Thread _RcvSndPktThread;
        Packet AllPacket;

        public ServerForm()
        {
            InitializeComponent();
            AllInfo = new PersonalInfo();//所有人的資訊
            //多人連線
            _ServerSocket = new ServerSocket();
            _ServerSocket.Bind();
            _ServerSocket.Listen();
            //Accept Thread
            _ServerSocket._AcceptThread = new Thread(_ServerSocket.AcceptThread);
            _ServerSocket._AcceptThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _ServerSocket._ClientSockets.Capacity; i++)
            {
                try
                {
                    if (_ServerSocket._ClientSockets[i].Connected)
                    {
                        MessageBox.Show("Client" + i + "連接成功");
                    }
                }
                catch
                {
                    break;
                }
            }
            this.Close();
            this.Dispose();
        }

        private void GameStartButton_Click(object sender, EventArgs e)
        {
            AllInfo.MaxPlayer = _ServerSocket._ClientSockets.Count;//取得玩家總數
            
            _ReceiveInfoThread = new Thread(ReceiveInfoThread);
            _ReceiveInfoThread.Start();

            SendInfo();//傳送所有人的資訊給每個人
            SendPacket();//傳送地圖資訊給每個人

            _RcvSndPktThread = new Thread(RcvSndPktThread);
            _RcvSndPktThread.Start();
        }

        private void ReceiveInfoThread()
        {
            for (int i = 0; i < AllInfo.MaxPlayer; i++)//接收每個玩家的個人資料
            {
                PersonalInfo tempInfo = new PersonalInfo();
                Byte[] Buffer = new Byte[_ServerSocket._ClientSockets[i].SendBufferSize];
                _ServerSocket._ClientSockets[i].Receive(Buffer);
                tempInfo = AllInfo.BytesToPersonalInfo(Buffer);
                AllInfo.Characters[i] = tempInfo.MyCharacter;
                AllInfo.Names[i] = tempInfo.MyName;
            }
        }

        private void RcvSndPktThread()//接受與寄送遊戲過程中的資訊
        {
            for (int i = 0; i < AllInfo.MaxPlayer; i++)
            {
                Packet tempPkt = new Packet(i, AllInfo.MaxPlayer);
                Byte[] Buffer = new Byte[_ServerSocket._ClientSockets[i].SendBufferSize];
                _ServerSocket._ClientSockets[i].Receive(Buffer);
                tempPkt = AllPacket.BytesToPacket(Buffer);
                Array.Copy(tempPkt.MapIslands, AllPacket.MapIslands, tempPkt.MapIslands.Length);
                Array.Copy(tempPkt.IslandOwners, AllPacket.IslandOwners, tempPkt.IslandOwners.Length);
                Array.Copy(tempPkt.DiceOnIsland, AllPacket.DiceOnIsland, tempPkt.DiceOnIsland.Length);
                Array.Copy(tempPkt.OldMapIslands, AllPacket.OldMapIslands, tempPkt.OldMapIslands.Length);
                AllPacket.NowID = tempPkt.NowID;
                AllPacket.NextID = tempPkt.NextID;
                AllPacket.LastID = tempPkt.LastID;

                AllPacket.whoWin = tempPkt.whoWin;
                AllPacket.LastID = AllPacket.NowID;
                AllPacket.NowID = AllPacket.NextID;
               
                if (AllPacket.NextID == AllInfo.MaxPlayer - 1)
                {
                    AllPacket.NextID = 0;
                }
                else
                {
                    AllPacket.NextID++;
                }

            B:  for (int j = 0; j < AllInfo.MaxPlayer; j++)//寄送遊戲進程中的資訊給每位玩家
                {
                    if (j != AllPacket.LastID)
                    {
                        try
                        {
                            _ServerSocket._ClientSockets[j].Send(AllPacket.PacketToBytes());
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            goto B;
                        }
                    }
                }

                if (i == AllInfo.MaxPlayer - 1)
                {
                    i = -1;
                }
            }
        }

        private void SendInfo()//寄送所有人的資訊給所每個人
        {
            foreach (Socket clientSocket in _ServerSocket._ClientSockets)
            {
                while (true)
                {
                    try
                    {
                        if (AllInfo.Names[AllInfo.MaxPlayer - 1] != null)
                        {
                            clientSocket.Send(AllInfo.PersonalInfoToBytes());
                            break;
                        }
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }
            }
        }

        private void SendPacket()//寄送初始化地圖資訊給每個人
        {
            AllPacket = new Packet(AllInfo.MaxPlayer);
        A:  foreach (Socket clientSocket in _ServerSocket._ClientSockets)
            {                
                try
                {
                    clientSocket.Send(AllPacket.PacketToBytes());
                }
                catch
                {
                    Thread.Sleep(1000);
                    goto A;
                }
                
            }
        }
    }
}

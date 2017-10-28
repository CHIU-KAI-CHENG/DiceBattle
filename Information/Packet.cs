using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Information
{
    public enum 角色
    {
        國王,
        皇后,
        王子,
        公主
    }

    [Serializable]
    public class Packet
    {
        Random rand = new Random();
        public int[,] OldMapIslands;//上一次的島嶼資訊
        public int[,] MapIslands;//座標上的每個島嶼是誰的
        public int[] IslandOwners;//每個玩家總共有幾個島
        public int[,] DiceOnIsland;//每個島上所有的骰子
        public int NextID;
        public int NowID;
        public int LastID;
        public int whoWin;

        public Packet(int ID,int MaxPlayer)
        {
            OldMapIslands = new int[4, 6];
            MapIslands = new int[4, 6];
            IslandOwners = new int[MaxPlayer];
            DiceOnIsland = new int[4, 6];
            NextID = 1;
            NowID = 0;
            LastID = -1;
            whoWin = -1;
        }

        public Packet(int MaxPlayer)
        {
            whoWin = -1;
            NextID = 1;
            NowID = 0;
            LastID = -1;
            OldMapIslands = new int[4, 6];
            MapIslands = new int[4, 6];
            IslandOwners = new int[MaxPlayer];
            DiceOnIsland = new int[4, 6];
            //歸零每個人所有的島嶼數
            for(int i=0;i<MaxPlayer;i++)
            {
                IslandOwners[i]=0;
            }
            //隨機分配島嶼
            for (int i = 0; i < MaxPlayer; i++)
            {
                for (int j = 0; j < 24 / MaxPlayer; j++)
                {
                    int tmpIndex1 = rand.Next(4);
                    int tmpIndex2 = rand.Next(6);
                    if (MapIslands[tmpIndex1, tmpIndex2] == 0)
                    {
                        MapIslands[tmpIndex1, tmpIndex2] = i;
                        IslandOwners[i]++;
                    }
                    else
                    {
                        j--;
                    }
                }
            }
            for (int i = 0; i < 24 % MaxPlayer; i++)
            {
                int tmpIndex1 = rand.Next(4);
                int tmpIndex2 = rand.Next(6);
                if (MapIslands[tmpIndex1, tmpIndex2] == 0)
                {
                    MapIslands[tmpIndex1, tmpIndex2] = i;
                    IslandOwners[i]++;
                }
                else
                {
                    i--;
                }
            }//隨機分配島嶼結束  
            //每塊地分一個骰子
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    DiceOnIsland[i, j] = 1;
                }
            }
            int[] EachDiceNum = new int[MaxPlayer];//每個人在每塊地分一個骰子後剩下的總骰子數
            for (int i = 0; i < MaxPlayer; i++)//設定每個人剩下的總骰子數
            {
                if (MaxPlayer != 5)
                {
                    EachDiceNum[i] = (24 / MaxPlayer) * 2;
                }
                else
                {
                    if (i != 4)
                    {
                        EachDiceNum[i] = 10;
                    }
                    else
                    {
                        EachDiceNum[i] = 11;
                    }
                }
            }
            //隨機產生每個島的骰子數
            for (int k = 0; k < MaxPlayer; k++)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (MapIslands[i, j] == k)
                        {
                            int rndNum = rand.Next(EachDiceNum[k] + 1);
                            if (rndNum > 6)
                            {
                                j--;
                                continue;
                            }
                            DiceOnIsland[i, j] += rndNum;
                            EachDiceNum[k] -= rndNum;
                        }
                    }
                }           
            }
            for (int i = 0; i < MaxPlayer; i++) //剩下的骰子補到每人最後一座島上
            {
                if (EachDiceNum[i] > 0)
                {
                    for (int j = 3; j >= 0; j--)
                    {
                        for (int k = 5; k >= 0; k--)
                        {
                            if (MapIslands[j, k] == i)
                            {
                                DiceOnIsland[j, k] += EachDiceNum[i];
                            }
                            break;
                        }
                        break;
                    }
                }
            }//隨機產生每個島的骰子數結束
            //將每個島的骰子數移給OldMapIslands
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    OldMapIslands[i, j] = MapIslands[i, j];
                }
            }
        }

        public Packet BytesToPacket(Byte[] PacketBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(PacketBytes);

            Packet packet = (Packet)bf.Deserialize(ms);
            ms.Close();
            return packet;
        }

        public Byte[] PacketToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            
            bf.Serialize(ms, this);
            byte[] Bytes = ms.ToArray();
            ms.Close();
            return Bytes;
        }
    }

    [Serializable]
    public class PersonalInfo
    {
        public int MaxPlayer { get; set; }//玩家人數上限
        public 角色 MyCharacter { get; set; }//自己的角色
        public string MyName { get; set; }//自己的名稱
        public 角色[] Characters { get; set; }//所有玩家的角色
        public string[] Names { get; set; }//所有玩家的名稱

        public PersonalInfo()
        {
            
            Characters = new 角色[6];
            Names = new string[6];
            MyCharacter = 角色.國王;//角色預設為國王
        }

        public PersonalInfo BytesToPersonalInfo(Byte[] PersonalInfoBytes)//將Byte[]轉換成PersonalInfo
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(PersonalInfoBytes);

            PersonalInfo personalInfo = (PersonalInfo)bf.Deserialize(ms);
            ms.Close();
            return personalInfo;
        }

        public Byte[] PersonalInfoToBytes()//將PersonalInfo轉換成Byte[]
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this);
            byte[] Bytes = ms.ToArray();
            ms.Close();
            return Bytes;
        }
    }

    //class Island
}

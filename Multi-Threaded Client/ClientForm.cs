using Information;
using Multi_Threaded_Client.Networking;
using Multi_Threaded_Client.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_Threaded_Client
{
    public partial class ClientForm : Form
    {        
        ClientSocket _ClientSocket;
        SoundPlayer MusicPlayer;
        Label LoadingLabel;
        Label[,] DiceOnIsland;
        Packet MyPacket ;
        PersonalInfo MyInfo = new PersonalInfo();
        Button NextTurn;//換人按鈕
        Button Battle;//對戰按鈕
        PictureBox[,] Flags;
        PictureBox[,] Islands;
        int myclickcancel=0,enemyclickcancel=0;
        PictureBox[] AttackDice=new PictureBox [8];
        PictureBox[] DefenseDice = new PictureBox[8];
        Button EndTurn;
        

        BackgroundWorker[] SndRcvPktInGame;

        int playerx, playery, enemyx, enemyy;

        int ID;//表示自己的順位

        
        

        public ClientForm()
        {
            
            InitializeComponent();
            ID = -1;
            MusicPlayer = new SoundPlayer();
            MusicPlayer.SoundLocation = "LoginMusic.wav";
            MusicPlayer.PlayLooping();
            
        }

        private void LeaveButton_Click(object sender, EventArgs e)//離開遊戲
        {
            this.Close();
            this.Dispose();
        }

        private void LoginButton_Click(object sender, EventArgs e)//登入遊戲
        {
            MyInfo.MyName = NameTextBox.Text;
            //連線至server
            _ClientSocket = new ClientSocket();
            try
            {
                
                _ClientSocket.Connect(IPTextBox.Text);
                MusicPlayer.Stop();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString() + "\n\n 請嘗試重新連接");
                return;
            }
            //捨棄登入控制項
            Loading();
            ReceiveAllInitialInfo.RunWorkerAsync();//接收所有初始化的資訊          
        }

        private void CharacterRightPictureBox_Click(object sender, EventArgs e)//按下右選單選角色
        {
            switch (MyInfo.MyCharacter)
            {
                case 角色.國王:
                    MyInfo.MyCharacter = 角色.皇后;
                    CharacterPictureBox.Image = Resources.皇后;
                    NameTextBox.Text = "皇后";
                    break;
                case 角色.皇后:
                    MyInfo.MyCharacter = 角色.王子;
                    CharacterPictureBox.Image = Resources.王子;
                    NameTextBox.Text = "王子";
                    break;
                case 角色.王子:
                    MyInfo.MyCharacter = 角色.公主;
                    CharacterPictureBox.Image = Resources.公主;
                    NameTextBox.Text = "公主";
                    break;
                case 角色.公主:
                    MyInfo.MyCharacter = 角色.國王;
                    CharacterPictureBox.Image = Resources.國王;
                    NameTextBox.Text = "國王";
                    break;
            }
        }

        private void CharacterLeftPictureBox_Click(object sender, EventArgs e)//按下左選單選角色
        {
            switch (MyInfo.MyCharacter)
            {
                case 角色.國王:
                    MyInfo.MyCharacter = 角色.公主;
                    CharacterPictureBox.Image = Resources.公主;
                    NameTextBox.Text = "公主";
                    break;
                case 角色.皇后:
                    MyInfo.MyCharacter = 角色.國王;
                    CharacterPictureBox.Image = Resources.國王;
                    NameTextBox.Text = "國王";
                    break;
                case 角色.王子:
                    MyInfo.MyCharacter = 角色.皇后;
                    CharacterPictureBox.Image = Resources.皇后;
                    NameTextBox.Text = "皇后";
                    break;
                case 角色.公主:
                    MyInfo.MyCharacter = 角色.王子;
                    CharacterPictureBox.Image = Resources.王子;
                    NameTextBox.Text = "王子";
                    break;
            }
        }

        private void EndTrnBtn_Click(object sender, EventArgs e)//換人按鈕觸發事件
        {
            /*if (CheckWin())
            {
                MessageBox.Show(MyInfo.Names[MyPacket.whoWin] + "勝利");
            }*/
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Random rnd = new Random(Guid.NewGuid().GetHashCode());
                    double h1 = rnd.NextDouble();
                    if (MyPacket.MapIslands[i, j] == ID)
                    {

                        if (h1 > 0.5 && MyPacket.DiceOnIsland[i, j] < 8) MyPacket.DiceOnIsland[i, j]++;
                    }
                    DiceOnIsland[i, j].Text = "" + MyPacket.DiceOnIsland[i, j];
                    Islands[i, j].Enabled = false;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                AttackDice[i].Visible = false;
                DefenseDice[i].Visible = false;
            }
            Battle.Text = "Battle";
            Battle.Enabled = false;
            EndTurn.Enabled = false;
            SendPkt();
            for (int i = 0; i < MyInfo.MaxPlayer - 1; i++)
            {
                SndRcvPktInGame[i].RunWorkerAsync();
            }
            
        }

        private void TextChanged(object sender, EventArgs e)//啟用登入按鈕
        {
            if (IPTextBox.Text != ""&& NameTextBox.Text != "")
            {
                LoginButton.Enabled = true;
            }
            //NameTextBox.Enter += LoginButton_Click;//按下Enter可以直接登入
        }

        private void ReceiveIDThread()//接收ID的Thread
        {
            Byte[] Buffer = new Byte[_ClientSocket._ClientSocket.SendBufferSize];
            _ClientSocket._ClientSocket.Receive(Buffer);
            ID=BitConverter.ToInt32(Buffer, 0);
        }

        private void ClientForm_SizeChanged(object sender, EventArgs e)
        {
            //foreach(object Object in )
            MessageBox.Show("Change");
        }

        private void Island_MouseMove(object sender, EventArgs e)  //滑鼠移到島上,島會變大
        {
            PictureBox chooseIsland = (PictureBox)sender;
            chooseIsland.Size = new Size(150, 105);

        }

        private void Island_MouseLeave(object sender, EventArgs e) //滑鼠離開島,島會回復正常大小
        {
            PictureBox chooseIsland = (PictureBox)sender;
            chooseIsland.Size = new Size(120, 90);

        }

        private void Island_Click(object sender, EventArgs e)  //點擊島嶼,會顯示
        {
            NextTurn.Enabled = true;

            PictureBox chooseIsland = (PictureBox)sender;
            chooseIsland.Size = new Size(150, 105);

            //如何取出i,j
            playerx = GetIslandIndexX(chooseIsland.Location.Y);
            playery = GetIslandIndexY(chooseIsland.Location.X);

            chooseIsland.MouseLeave += Island_MouseLeave;

            //隱藏上次戰鬥結果
            for (int i = 0; i < 8; i++)
            {
                AttackDice[i].Visible = false;
            }

            if (myclickcancel == 0) //還沒按過,用clickcencel
            {
                BeforeCancel(playerx, playery);
                myclickcancel++;

                SoundPlayer SelectMusic = new SoundPlayer();
                SelectMusic.SoundLocation = "SelectMusic.wav";
                SelectMusic.Play();

                Islands[playerx, playery].Image = Resources.Select;  //換成選中島嶼圖


                for (int i = 0; i < MyPacket.DiceOnIsland[playerx, playery]; i++)
                {
                    AttackDice[i].Visible = true;
                    switch (ID)
                    {
                        case 0:
                            AttackDice[i].Image = Resources.紅骰子;
                            break;
                        case 1:
                            AttackDice[i].Image = Resources.黃骰子;
                            break;
                        case 2:
                            AttackDice[i].Image = Resources.藍骰子;
                            break;
                        case 3:
                            AttackDice[i].Image = Resources.綠骰子;
                            break;
                        case 4:
                            AttackDice[i].Image = Resources.紫骰子;
                            break;
                        case 5:
                            AttackDice[i].Image = Resources.白骰子;
                            break;
                    }

                }
            }
            else //已經按過一次了 取消clickcencel
            {
                SoundPlayer CancelMusic = new SoundPlayer();
                CancelMusic.SoundLocation = "CancelMusic.wav";
                CancelMusic.Play();
                BeforeAttack();
                CheckIsOne();

                myclickcancel = 0;
                Islands[playerx, playery].Image = Resources.island_1;  //換成普通島嶼圖

                for (int h = 0; h < MyPacket.DiceOnIsland[playerx, playery]; h++)
                {


                    AttackDice[h].Visible = false;
                }
            }

            Battle.Text = "Battle";
        }

        private void EnemyIsland_Click(object sender, EventArgs e)  //點擊敵人島嶼,會顯示
        {

            PictureBox chooseIsland = (PictureBox)sender;
            chooseIsland.Size = new Size(150, 105);

            //如何取出i,j
            enemyx = GetIslandIndexX(chooseIsland.Location.Y);
            enemyy = GetIslandIndexY(chooseIsland.Location.X);

            chooseIsland.MouseLeave += Island_MouseLeave;

            //隱藏上次戰鬥結果
            for (int i = 0; i < 8; i++)
            {
                DefenseDice[i].Visible = false;
            }


            if (enemyclickcancel == 0) //還沒按過,用clickcencel
            {
                BeforeCancel(enemyx, enemyy);
                enemyclickcancel++;

                Battle.Enabled = true;

                SoundPlayer SelectMusic = new SoundPlayer();
                SelectMusic.SoundLocation = "SelectMusic.wav";
                SelectMusic.Play();

                Islands[enemyx, enemyy].Image = Resources.Select2;

                for (int i = 0; i < MyPacket.DiceOnIsland[enemyx, enemyy]; i++)
                {


                    DefenseDice[i].Visible = true;

                    switch (MyPacket.MapIslands[enemyx, enemyy])
                    {
                        case 0:
                            DefenseDice[i].Image = Resources.紅骰子;
                            break;
                        case 1:
                            DefenseDice[i].Image = Resources.黃骰子;
                            break;
                        case 2:
                            DefenseDice[i].Image = Resources.藍骰子;
                            break;
                        case 3:
                            DefenseDice[i].Image = Resources.綠骰子;
                            break;
                        case 4:
                            DefenseDice[i].Image = Resources.紫骰子;
                            break;
                        case 5:
                            DefenseDice[i].Image = Resources.白骰子;
                            break;
                    }

                }
                BeforeBattle(enemyx, enemyy);

            }
            else //已經按過一次了 取消clickcencel
            {
                SoundPlayer CancelMusic = new SoundPlayer();
                CancelMusic.SoundLocation = "CancelMusic.wav";
                CancelMusic.Play();
                BeforeAttack();
                CheckIsOne();
                enemyclickcancel = 0;
                Islands[enemyx, enemyy].Image = Resources.island_1;

                BeforeCancel(playerx, playery);

                Battle.Enabled = false;

                for (int h = 0; h < MyPacket.DiceOnIsland[enemyx, enemyy]; h++)
                {

                    DefenseDice[h].Visible = false;
                }
            }


        }

        private void Battle_Click(object sender, EventArgs e)
        {
            Random num = new Random(Guid.NewGuid().GetHashCode());

            int AttackPower = 0;
            for (int i = 0; i < MyPacket.DiceOnIsland[playerx, playery]; i++)
            {

                AttackDice[i].BackColor = Color.Transparent;

                //如何取出i,j
                switch (MyPacket.MapIslands[playerx, playery])
                {
                    case 0:
                        int x0 = num.Next(1, 7);
                        switch (x0)
                        {
                            case 1: AttackDice[i].Image = Resources.DiceR1; AttackPower += 1; break;
                            case 2: AttackDice[i].Image = Resources.DiceR2; AttackPower += 2; break;
                            case 3: AttackDice[i].Image = Resources.DiceR3; AttackPower += 3; break;
                            case 4: AttackDice[i].Image = Resources.DiceR4; AttackPower += 4; break;
                            case 5: AttackDice[i].Image = Resources.DiceR5; AttackPower += 5; break;
                            case 6: AttackDice[i].Image = Resources.DiceR6; AttackPower += 6; break;
                        } break;
                    case 1:
                        int x1 = num.Next(1, 7);
                        switch (x1)
                        {
                            case 1: AttackDice[i].Image = Resources.DiceY1; AttackPower += 1; break;
                            case 2: AttackDice[i].Image = Resources.DiceY2; AttackPower += 2; break;
                            case 3: AttackDice[i].Image = Resources.DiceY3; AttackPower += 3; break;
                            case 4: AttackDice[i].Image = Resources.DiceY4; AttackPower += 4; break;
                            case 5: AttackDice[i].Image = Resources.DiceY5; AttackPower += 5; break;
                            case 6: AttackDice[i].Image = Resources.DiceY6; AttackPower += 6; break;
                        } break;
                    case 2:
                        int x2 = num.Next(1, 7);
                        switch (x2)
                        {
                            case 1: AttackDice[i].Image = Resources.DiceB1; AttackPower += 1; break;
                            case 2: AttackDice[i].Image = Resources.DiceB2; AttackPower += 2; break;
                            case 3: AttackDice[i].Image = Resources.DiceB3; AttackPower += 3; break;
                            case 4: AttackDice[i].Image = Resources.DiceB4; AttackPower += 4; break;
                            case 5: AttackDice[i].Image = Resources.DiceB5; AttackPower += 5; break;
                            case 6: AttackDice[i].Image = Resources.DiceB6; AttackPower += 6; break;
                        } break;
                    case 3:
                        int x3 = num.Next(1, 7);
                        switch (x3)
                        {
                            case 1: AttackDice[i].Image = Resources.DiceG1; AttackPower += 1; break;
                            case 2: AttackDice[i].Image = Resources.DiceG2; AttackPower += 2; break;
                            case 3: AttackDice[i].Image = Resources.DiceG3; AttackPower += 3; break;
                            case 4: AttackDice[i].Image = Resources.DiceG4; AttackPower += 4; break;
                            case 5: AttackDice[i].Image = Resources.DiceG5; AttackPower += 5; break;
                            case 6: AttackDice[i].Image = Resources.DiceG6; AttackPower += 6; break;
                        } break;
                    case 4:
                        int x4 = num.Next(1, 7);
                        switch (x4)
                        {
                            case 1: AttackDice[i].Image = Resources.DiceP1; AttackPower += 1; break;
                            case 2: AttackDice[i].Image = Resources.DiceP2; AttackPower += 2; break;
                            case 3: AttackDice[i].Image = Resources.DiceP3; AttackPower += 3; break;
                            case 4: AttackDice[i].Image = Resources.DiceP4; AttackPower += 4; break;
                            case 5: AttackDice[i].Image = Resources.DiceP5; AttackPower += 5; break;
                            case 6: AttackDice[i].Image = Resources.DiceP6; AttackPower += 6; break;
                        } break;
                    case 5:
                        int x5 = num.Next(1, 7);
                        switch (x5)
                        {
                            case 1: AttackDice[i].Image = Resources.DiceW1; AttackPower += 1; break;
                            case 2: AttackDice[i].Image = Resources.DiceW2; AttackPower += 2; break;
                            case 3: AttackDice[i].Image = Resources.DiceW3; AttackPower += 3; break;
                            case 4: AttackDice[i].Image = Resources.DiceW4; AttackPower += 4; break;
                            case 5: AttackDice[i].Image = Resources.DiceW5; AttackPower += 5; break;
                            case 6: AttackDice[i].Image = Resources.DiceW6; AttackPower += 6; break;
                        } break;
                }
                AttackDice[i].Location = new Point(100 + i * 60, 440);
                AttackDice[i].Name = "DefenseDice" + i;
                AttackDice[i].Size = new Size(40, 40);
                AttackDice[i].SizeMode = PictureBoxSizeMode.StretchImage;
                AttackDice[i].TabStop = false;
                AttackDice[i].BringToFront();

            }

            int DefensePower = 0;
            for (int i = 0; i < MyPacket.DiceOnIsland[enemyx, enemyy]; i++)
            {

                DefenseDice[i].BackColor = Color.Transparent;

                //如何取出i,j
                switch (MyPacket.MapIslands[enemyx, enemyy])
                {
                    case 0:
                        int x0 = num.Next(1, 7);
                        switch (x0)
                        {
                            case 1: DefenseDice[i].Image = Resources.DiceR1; DefensePower += 1; break;
                            case 2: DefenseDice[i].Image = Resources.DiceR2; DefensePower += 2; break;
                            case 3: DefenseDice[i].Image = Resources.DiceR3; DefensePower += 3; break;
                            case 4: DefenseDice[i].Image = Resources.DiceR4; DefensePower += 4; break;
                            case 5: DefenseDice[i].Image = Resources.DiceR5; DefensePower += 5; break;
                            case 6: DefenseDice[i].Image = Resources.DiceR6; DefensePower += 6; break;
                        } break;
                    case 1:
                        int x1 = num.Next(1, 7);
                        switch (x1)
                        {
                            case 1: DefenseDice[i].Image = Resources.DiceY1; DefensePower += 1; break;
                            case 2: DefenseDice[i].Image = Resources.DiceY2; DefensePower += 2; break;
                            case 3: DefenseDice[i].Image = Resources.DiceY3; DefensePower += 3; break;
                            case 4: DefenseDice[i].Image = Resources.DiceY4; DefensePower += 4; break;
                            case 5: DefenseDice[i].Image = Resources.DiceY5; DefensePower += 5; break;
                            case 6: DefenseDice[i].Image = Resources.DiceY6; DefensePower += 6; break;
                        } break;
                    case 2:
                        int x2 = num.Next(1, 7);
                        switch (x2)
                        {
                            case 1: DefenseDice[i].Image = Resources.DiceB1; DefensePower += 1; break;
                            case 2: DefenseDice[i].Image = Resources.DiceB2; DefensePower += 2; break;
                            case 3: DefenseDice[i].Image = Resources.DiceB3; DefensePower += 3; break;
                            case 4: DefenseDice[i].Image = Resources.DiceB4; DefensePower += 4; break;
                            case 5: DefenseDice[i].Image = Resources.DiceB5; DefensePower += 5; break;
                            case 6: DefenseDice[i].Image = Resources.DiceB6; DefensePower += 6; break;
                        } break;
                    case 3:
                        int x3 = num.Next(1, 7);
                        switch (x3)
                        {
                            case 1: DefenseDice[i].Image = Resources.DiceG1; DefensePower += 1; break;
                            case 2: DefenseDice[i].Image = Resources.DiceG2; DefensePower += 2; break;
                            case 3: DefenseDice[i].Image = Resources.DiceG3; DefensePower += 3; break;
                            case 4: DefenseDice[i].Image = Resources.DiceG4; DefensePower += 4; break;
                            case 5: DefenseDice[i].Image = Resources.DiceG5; DefensePower += 5; break;
                            case 6: DefenseDice[i].Image = Resources.DiceG6; DefensePower += 6; break;
                        } break;
                    case 4:
                        int x4 = num.Next(1, 7);
                        switch (x4)
                        {
                            case 1: DefenseDice[i].Image = Resources.DiceP1; DefensePower += 1; break;
                            case 2: DefenseDice[i].Image = Resources.DiceP2; DefensePower += 2; break;
                            case 3: DefenseDice[i].Image = Resources.DiceP3; DefensePower += 3; break;
                            case 4: DefenseDice[i].Image = Resources.DiceP4; DefensePower += 4; break;
                            case 5: DefenseDice[i].Image = Resources.DiceP5; DefensePower += 5; break;
                            case 6: DefenseDice[i].Image = Resources.DiceP6; DefensePower += 6; break;
                        } break;
                    case 5:
                        int x5 = num.Next(1, 7);
                        switch (x5)
                        {
                            case 1: DefenseDice[i].Image = Resources.DiceW1; DefensePower += 1; break;
                            case 2: DefenseDice[i].Image = Resources.DiceW2; DefensePower += 2; break;
                            case 3: DefenseDice[i].Image = Resources.DiceW3; DefensePower += 3; break;
                            case 4: DefenseDice[i].Image = Resources.DiceW4; DefensePower += 4; break;
                            case 5: DefenseDice[i].Image = Resources.DiceW5; DefensePower += 5; break;
                            case 6: DefenseDice[i].Image = Resources.DiceW6; DefensePower += 6; break;
                        } break;
                }
                DefenseDice[i].Location = new Point(610 + i * 60, 440);
                DefenseDice[i].Name = "DefenseDice" + i;
                DefenseDice[i].Size = new Size(40, 40);
                DefenseDice[i].SizeMode = PictureBoxSizeMode.StretchImage;
                DefenseDice[i].TabStop = false;
                this.Controls.Add(DefenseDice[i]);
                DefenseDice[i].BringToFront();

            }

            Battle.Text = AttackPower + "vs" + DefensePower;
            SoundPlayer BattleMusic = new SoundPlayer();
            AttackPower = 20;
            DefensePower = 2;
            if (AttackPower > DefensePower)
            {
                BattleMusic.SoundLocation = "BattleMusic.wav";
                BattleMusic.Play();
                MyPacket.MapIslands[enemyx, enemyy] = ID;
                MyPacket.DiceOnIsland[enemyx, enemyy] = (AttackPower - DefensePower) / 6 + 1;
                MyPacket.DiceOnIsland[playerx, playery] = 1;



                switch (MyPacket.MapIslands[enemyx, enemyy])
                {
                    case 0:
                        Flags[enemyx, enemyy].Image = Resources.紅旗;
                        break;
                    case 1:
                        Flags[enemyx, enemyy].Image = Resources.黃旗;
                        break;
                    case 2:
                        Flags[enemyx, enemyy].Image = Resources.藍旗;
                        break;
                    case 3:
                        Flags[enemyx, enemyy].Image = Resources.綠旗;
                        break;
                    case 4:
                        Flags[enemyx, enemyy].Image = Resources.紫旗;
                        break;
                    case 5:
                        Flags[enemyx, enemyy].Image = Resources.白旗;
                        break;
                }

                Islands[enemyx, enemyy].Click -= EnemyIsland_Click;
                Islands[enemyx, enemyy].Click += Island_Click;
                DrawFlags();
                DrawLabel();
            }
            else
            {

                BattleMusic.SoundLocation = "LoseMusic.wav";
                BattleMusic.Play();
                MyPacket.DiceOnIsland[playerx, playery] = 1;
                DrawFlags();
                DrawLabel();
            }

            BeforeAttack();
            CheckIsOne();

            myclickcancel = 0;
            enemyclickcancel = 0;
            Islands[playerx, playery].Image = Resources.island_1;
            Islands[enemyx, enemyy].Image = Resources.island_1;

            Battle.Enabled = false;

            if (CheckWin())
            {
                SoundPlayer WinPlayer = new SoundPlayer();
                MusicPlayer.SoundLocation = "WinMusic.wav";
                MusicPlayer.PlayLooping();
                //MusicPlayer.Stop();
                SendPkt();
                MessageBox.Show(MyInfo.Names[MyPacket.whoWin] + "佔據了所有島嶼，恭喜你成為ＤＢ之王");
                
                this.Close();
            }

        }//按了BATTLE後要顯示出雙方的骰子點數,並戰鬥

      

        private void SendInfo()//傳送個人資料
        {
            _ClientSocket._ClientSocket.Send(MyInfo.PersonalInfoToBytes());
        }

        private void SendPkt()//傳送遊戲資料
        {
            _ClientSocket._ClientSocket.Send(MyPacket.PacketToBytes());
        }

        private void ReceiveAllInfoThread()//接收所有人資料的
        {
            PersonalInfo tempInfo = new PersonalInfo();
            Byte[] Buffer = new Byte[_ClientSocket._ClientSocket.SendBufferSize];
            
            _ClientSocket._ClientSocket.Receive(Buffer);
            tempInfo = MyInfo.BytesToPersonalInfo(Buffer);
            Array.Copy(tempInfo.Characters, MyInfo.Characters, tempInfo.Characters.Length);
            Array.Copy(tempInfo.Names, MyInfo.Names, tempInfo.Names.Length);
            MyInfo.MaxPlayer = tempInfo.MaxPlayer;
            //初始化個人的packet
            MyPacket = new Packet(ID, MyInfo.MaxPlayer);
            //建構接收遊戲資訊的BackgroundWorker陣列
            SndRcvPktInGame = new BackgroundWorker[MyInfo.MaxPlayer];
            for (int i = 0; i < MyInfo.MaxPlayer; i++)
            {
                SndRcvPktInGame[i] = new BackgroundWorker();
                SndRcvPktInGame[i].DoWork += SndRcvPktInGame_DoWork;
                SndRcvPktInGame[i].RunWorkerCompleted += SndRcvPktInGame_RunWorkerCompleted;
            }
        }

        private void ReceiveAllPacketThread()//接收地圖資訊
        {
            Packet tempPkt = new Packet(ID,MyInfo.MaxPlayer);
            MyPacket = new Packet(ID, MyInfo.MaxPlayer);
            Byte[] Buffer = new Byte[_ClientSocket._ClientSocket.SendBufferSize];
            _ClientSocket._ClientSocket.Receive(Buffer);
            tempPkt = MyPacket.BytesToPacket(Buffer);
            Array.Copy(tempPkt.MapIslands, MyPacket.MapIslands, tempPkt.MapIslands.Length);
            Array.Copy(tempPkt.IslandOwners, MyPacket.IslandOwners, tempPkt.IslandOwners.Length);
            Array.Copy(tempPkt.DiceOnIsland, MyPacket.DiceOnIsland, tempPkt.DiceOnIsland.Length);
            Array.Copy(tempPkt.OldMapIslands, MyPacket.OldMapIslands, tempPkt.OldMapIslands.Length);
            MyPacket.NowID = tempPkt.NowID;
            MyPacket.NextID = tempPkt.NextID;
            MyPacket.LastID = tempPkt.LastID;
            MyPacket.whoWin = tempPkt.whoWin;
            if (MyPacket.whoWin != -1)
            {
                SoundPlayer LosePlayer = new SoundPlayer();
                MusicPlayer.SoundLocation = "DeathMusic.wav";
                MusicPlayer.PlayLooping();
                timer1.Start();
                //MusicPlayer.Stop();
                MessageBox.Show(MyInfo.Names[MyPacket.whoWin] + "佔據了所有島嶼，你輸了！");
                this.Close();
            }
        }
        
        private void Loading()//關閉原來的控制項
        {
            InputIPLabel.Dispose();
            NameLabel.Dispose();
            IPTextBox.Dispose();
            NameTextBox.Dispose();
            LoginButton.Dispose();
            LeaveButton.Dispose();
            ChooseCharacterLabel.Dispose();
            CharacterLeftPictureBox.Dispose();
            CharacterRightPictureBox.Dispose();
            CharacterPictureBox.Dispose();
            //產生Loading標籤
            LoadingLabel = new Label();
            LoadingLabel.AutoSize = true;
            LoadingLabel.Font = new System.Drawing.Font("微軟正黑體", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            LoadingLabel.Location = new System.Drawing.Point(527, 578);
            LoadingLabel.Name = "LoadingLabeI";
            LoadingLabel.Size = new System.Drawing.Size(192, 47);
            LoadingLabel.TabIndex = 7;
            LoadingLabel.Text = "Loading...";
            this.Controls.Add(LoadingLabel);
            LoadingLabel.BringToFront();
        }

        private void GameStartView()//遊戲開始畫面
        {
            this.Text = MyInfo.MyName + "的骰子戰爭        " + MyInfo.MaxPlayer + "人遊戲"; //AAA會顯示玩家的名字在視窗標題
            LoadingLabel.Dispose();
            
            PictureBox[] PlayerPhotos = new PictureBox[6];//玩家照片陣列

            Label[] PlayerNames = new Label[6];//玩家姓名陣列
            PictureBox[,] cStones = new PictureBox[4, 15];//橫的石頭
            PictureBox[] PlayerDice = new PictureBox[6];//每個玩家的骰子 
            Label ThePlayer = new Label();
            //產生每個玩家的照片
            for (int i = 0; i < MyInfo.MaxPlayer; i++)
            {
                PlayerPhotos[i] = new PictureBox();
                switch (MyInfo.Characters[i])
                {
                    case 角色.國王:
                        PlayerPhotos[i].Image = Resources.國王;
                        break;
                    case 角色.皇后:
                        PlayerPhotos[i].Image = Resources.皇后;
                        break;
                    case 角色.王子:
                        PlayerPhotos[i].Image = Resources.王子;
                        break;
                    case 角色.公主:
                        PlayerPhotos[i].Image = Resources.公主;
                        break;
                }
                PlayerPhotos[i].Location = new Point(10 + i * 200, 520);
                PlayerPhotos[i].Name = "PlayerPhotos" + i;
                PlayerPhotos[i].Size = new Size(80, 120);
                PlayerPhotos[i].SizeMode = PictureBoxSizeMode.StretchImage;
                PlayerPhotos[i].TabStop = false;
                this.Controls.Add(PlayerPhotos[i]);
                PlayerPhotos[i].BringToFront();
                //產生每個玩家的名稱標籤
                PlayerNames[i] = new Label();
                PlayerNames[i].AutoSize = true;
                PlayerNames[i].Font = new Font("微軟正黑體", 13.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                PlayerNames[i].Location = new Point(10 + i * 200, 640);
                PlayerNames[i].Name = "PlayerNames" + i;
                PlayerNames[i].Size = new Size(80, 60);
                PlayerNames[i].Text = MyInfo.Names[i];
                PlayerNames[i].TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(PlayerNames[i]);
                PlayerNames[i].BringToFront();
                //AAA產生當前玩家的標示物
                if (i == ID)
                {
                    
                    ThePlayer.AutoSize = true;
                    ThePlayer.Font = new Font("微軟正黑體", 13.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                    ThePlayer.Location = new Point(10 + i * 200, 490);
                    ThePlayer.Size = new Size(80, 60);
                    ThePlayer.Text = "you";
                    ThePlayer.TextAlign = ContentAlignment.MiddleCenter;
                    this.Controls.Add(ThePlayer);
                    ThePlayer.BringToFront();
                }
                //產生每個玩家的骰子
                PlayerDice[i] = new PictureBox();
                PlayerDice[i].BackColor = Color.Transparent;
                switch (i)
                {
                    case 0:
                        PlayerDice[i].Image = Resources.紅骰子;
                        break;
                    case 1:
                        PlayerDice[i].Image = Resources.黃骰子;
                        break;
                    case 2:
                        PlayerDice[i].Image = Resources.藍骰子;
                        break;
                    case 3:
                        PlayerDice[i].Image = Resources.綠骰子;
                        break;
                    case 4:
                        PlayerDice[i].Image = Resources.紫骰子;
                        break;
                    case 5:
                        PlayerDice[i].Image = Resources.白骰子;
                        break;
                }
                PlayerDice[i].Location = new Point(100 + i * 200, 560);
                PlayerDice[i].Name = "PlayerDice" + i;
                PlayerDice[i].Size = new Size(40, 40);
                PlayerDice[i].SizeMode = PictureBoxSizeMode.StretchImage;
                PlayerDice[i].TabStop = false;
                this.Controls.Add(PlayerDice[i]);
                PlayerDice[i].BringToFront();
            }
            //動態配置橫石頭
            for (int i = 0; i < 4; i++)
            {
                int newStart = 140;
                for (int j = 0; j < 3; j++)
                {
                    cStones[i, j] = new PictureBox();
                    cStones[i, j].BackgroundImage = Resources.image;
                    cStones[i, j].Location = new Point(newStart + j * 30, 50 + i * 100);
                    cStones[i, j].Image = Resources.stone;
                    cStones[i, j].Name = "cStones" + i + "," + j;
                    cStones[i, j].Size = new Size(20, 20);
                    cStones[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    cStones[i, j].TabStop = false;
                    this.Controls.Add(cStones[i, j]);
                    cStones[i, j].BringToFront();
                    if (j % 3 == 2)
                    {
                        newStart += 200;
                        j = -1;
                        if (newStart > 1000)
                        {
                            break;
                        }
                    }
                }
            }
            //布上島嶼
            DrawIslands();
            DrawFlags();//畫旗子
            DrawLabel();//表示島嶼的骰子數
            //產生換人按鈕
            CreatEndTurnBtn();
            
            //產生戰鬥按鈕
            CreatBattleBtn();
            //新增換人按鈕
            NextTurn = new Button();
            if (MyPacket.NowID == ID)
            {
               
                BeforeAttack();
                CheckIsOne();            
            }
            else
            {
                
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        Islands[i, j].Enabled = false;
                    }
                }
            }
            //產生戰鬥用骰子
            RenewDice();

            this.BackgroundImage = Resources.image;
            BackgroundPictureBox.Dispose();

            //不是第一輪的玩家準備接收資訊
            if (MyPacket.NowID != ID)
            {
                for (int i = 0; i < ID; i++)
                {
                    SndRcvPktInGame[i].RunWorkerAsync();
                }
            }
        }
    
        private void GameProcessView(bool IsMe) //遊戲進程
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {   //取消原有島嶼點擊事件
                    if (MyPacket.OldMapIslands[i, j] == ID)
                    {
                        Islands[i, j].Click -= Island_Click;
                    }
                    else
                    {
                        Islands[i, j].Click -= EnemyIsland_Click;
                    }
                    //新增現有島嶼點擊事件
                    if (MyPacket.MapIslands[i, j] == ID)
                    {
                        Islands[i, j].Click += Island_Click;
                    }
                    else
                    {
                        Islands[i, j].Click += EnemyIsland_Click;
                    }
                    //更改領地
                    switch(MyPacket.MapIslands[i, j])
                    {
                        case 0:
                            Flags[i, j].Image = Resources.紅旗;
                            DiceOnIsland[i, j].BackColor = Color.Red;
                            break;
                        case 1:
                            Flags[i, j].Image = Resources.黃旗;
                            DiceOnIsland[i, j].BackColor=Color.Yellow;
                            break;
                        case 2:
                            Flags[i, j].Image = Resources.藍旗;
                            DiceOnIsland[i, j].BackColor = Color.Blue;
                            break;
                        case 3:
                            Flags[i, j].Image = Resources.綠旗;
                            DiceOnIsland[i, j].BackColor = Color.Green;
                            break;
                        case 4:
                            Flags[i, j].Image = Resources.紫旗;
                            DiceOnIsland[i, j].BackColor = Color.Purple;
                            break;
                        case 5:
                            Flags[i, j].Image = Resources.白旗;
                            DiceOnIsland[i, j].BackColor = Color.White;
                            break;
                    }
                    DiceOnIsland[i, j].Text = "" + MyPacket.DiceOnIsland[i, j];
                    MyPacket.OldMapIslands[i, j] = MyPacket.MapIslands[i, j];
                }
            }

            if (IsMe)
            {
                BeforeAttack();
                CheckIsOne();
                EndTurn.Enabled = true;
            }
        }

        private void DrawFlags()//畫旗子(呼叫於GameStartView)
        {
            Flags = new PictureBox[4, 6];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Flags[i, j] = new PictureBox();
                    Flags[i, j].BackColor = Color.Transparent;
                    Flags[i, j].Location = new Point(50 + j * 200, i * 100);
                    switch (MyPacket.MapIslands[i, j])
                    {
                        case 0:
                            Flags[i, j].Image = Resources.紅旗;
                            break;
                        case 1:
                            Flags[i, j].Image = Resources.黃旗;
                            break;
                        case 2:
                            Flags[i, j].Image = Resources.藍旗;
                            break;
                        case 3:
                            Flags[i, j].Image = Resources.綠旗;
                            break;
                        case 4:
                            Flags[i, j].Image = Resources.紫旗;
                            break;
                        case 5:
                            Flags[i, j].Image = Resources.白旗;
                            break;
                    }
                    Flags[i, j].Name = "Flags" + i + "," + j;
                    Flags[i, j].Size = new Size(100, 50);
                    Flags[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    Flags[i, j].TabStop = false;
                    this.Controls.Add(Flags[i, j]);
                    Flags[i, j].BringToFront();
                }
            }
        }

        private void RenewDice()
        {
            for (int i = 0; i < 8; i++)
            {
                AttackDice[i] = new PictureBox();
              
                AttackDice[i].BackColor = Color.Transparent;
                AttackDice[i].Location = new Point(100 + i * 60, 440);
                AttackDice[i].Visible = false;
                AttackDice[i].Name = "AttackDice" + i;
                AttackDice[i].Size = new Size(40, 40);
                AttackDice[i].SizeMode = PictureBoxSizeMode.StretchImage;
                AttackDice[i].TabStop = false;
                this.Controls.Add(AttackDice[i]);
                AttackDice[i].BringToFront();
            }

            for (int i = 0; i < 8; i++)
            {

                DefenseDice[i] = new PictureBox();
              
                DefenseDice[i].BackColor = Color.Transparent;
                DefenseDice[i].Location = new Point(610 + i * 60, 440);
                DefenseDice[i].Visible = false;
                DefenseDice[i].Name = "DefenseDice" + i;
                DefenseDice[i].Size = new Size(40, 40);
                DefenseDice[i].SizeMode = PictureBoxSizeMode.StretchImage;
                DefenseDice[i].TabStop = false;
                this.Controls.Add(DefenseDice[i]);
                DefenseDice[i].BringToFront();
            }
        }

        private void DrawIslands()//畫島嶼(呼叫於GameStartView)
        {
            Islands = new PictureBox[4, 6];
            //動態配置島嶼
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {

                    Islands[i, j] = new PictureBox();
                    Islands[i, j].BackColor = Color.Transparent;
                    Islands[i, j].Location = new Point(20 + j * 200, 20 + i * 100);
                    Islands[i, j].Image = Resources.island_1;
                    Islands[i, j].Name = "Island" + i + "," + j;
                    Islands[i, j].Size = new Size(120, 90);
                    Islands[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    Islands[i, j].TabStop = false;
                    this.Controls.Add(Islands[i, j]);
                    Islands[i, j].BringToFront();

                    if (MyPacket.MapIslands[i, j] == ID)
                    {

                        Islands[i, j].Click += Island_Click;
                        Islands[i, j].Cursor = Cursors.Hand;
                        Islands[i, j].MouseMove += Island_MouseMove;
                        Islands[i, j].MouseLeave += Island_MouseLeave;

                    }
                    if (MyPacket.MapIslands[i, j] != ID)
                    {

                        Islands[i, j].Click += EnemyIsland_Click;
                        Islands[i, j].Cursor = Cursors.Hand;
                        Islands[i, j].MouseMove += Island_MouseMove;
                        Islands[i, j].MouseLeave += Island_MouseLeave;

                    }


                }
            }
        }

        private void DrawLabel()//標示每個島嶼上有幾個骰子的標籤(呼叫於GameStartView)
        {
            DiceOnIsland = new Label[4, 6];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    DiceOnIsland[i, j] = new Label();
                    DiceOnIsland[i, j].AutoSize = true;
                    switch (MyPacket.MapIslands[i, j])
                    {
                        case 0:
                            DiceOnIsland[i, j].BackColor = Color.Red;
                            break;
                        case 1:
                            DiceOnIsland[i, j].BackColor = Color.Yellow;
                            break;
                        case 2:
                            DiceOnIsland[i, j].BackColor = Color.Blue;
                            break;
                        case 3:
                            DiceOnIsland[i, j].BackColor = Color.Green;
                            break;
                        case 4:
                            DiceOnIsland[i, j].BackColor = Color.Purple;
                            break;
                        case 5:
                            DiceOnIsland[i, j].BackColor = Color.White;
                            break;
                    }
                    DiceOnIsland[i, j].Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                    DiceOnIsland[i, j].Location = new Point(90 + j * 200, 10 + i * 100);
                    DiceOnIsland[i, j].Name = "DiceOnIsland" + i + "," + j;
                    DiceOnIsland[i, j].TabStop = false;
                    DiceOnIsland[i, j].Text = "" + MyPacket.DiceOnIsland[i, j];
                    this.Controls.Add(DiceOnIsland[i, j]);
                    DiceOnIsland[i, j].BringToFront();
                }
            }

        }

        private void CreatBattleBtn()//產生Battle按鈕
        {
            Battle = new Button();
            Battle.BackColor = System.Drawing.SystemColors.ActiveCaption;
            Battle.Enabled = false;
            Battle.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            Battle.FlatStyle = FlatStyle.Popup;
            Battle.Location = new System.Drawing.Point(500, 440);
            Battle.Name = "Battle";
            Battle.Size = new System.Drawing.Size(106, 78);
            Battle.TabIndex = 7;
            Battle.Text = "Battle";
            Battle.UseVisualStyleBackColor = false;
            this.Controls.Add(Battle);
            Battle.BringToFront();
            Battle.Click += Battle_Click;
        }

        private void CreatEndTurnBtn()//產生換人按鈕
        {
            EndTurn = new Button();
            EndTurn.BackColor = System.Drawing.SystemColors.ActiveCaption;
            EndTurn.Enabled = false;
            EndTurn.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            EndTurn.FlatStyle = FlatStyle.Popup;
            EndTurn.Location = new System.Drawing.Point(1090, 400);
            EndTurn.Name = "EndTurn";
            EndTurn.Size = new System.Drawing.Size(106, 78);
            EndTurn.TabIndex = 7;
            EndTurn.Text = "End\nTurn";
            EndTurn.UseVisualStyleBackColor = false;
            this.Controls.Add(EndTurn);
            EndTurn.BringToFront();
            EndTurn.Click += EndTrnBtn_Click;
            if (ID == 0)
            {
                EndTurn.Enabled = true;
            }
        }



        private int GetIslandIndexX(int X)//取得島嶼x座標
        {
            return (X - 20) / 100;
        }

        private int GetIslandIndexY(int Y)//取得島嶼y座標
        {
            return (Y - 20) / 200;
        }

        private void CheckIsOne()//自己骰子若是1則不能點擊
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (MyPacket.MapIslands[i, j] == ID)
                    {
                        if (MyPacket.DiceOnIsland[i, j] == 1)
                        {
                            Islands[i, j].Enabled = false;
                        }
                    }
                }
            }
        }

        private void AddIslandEvent() 
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (MyPacket.MapIslands[i, j] == ID)
                    {

                        Islands[i, j].Click += Island_Click;
                        Islands[i, j].Cursor = Cursors.Hand;
                        Islands[i, j].MouseMove += Island_MouseMove;
                        Islands[i, j].MouseLeave += Island_MouseLeave;

                    }
                    if (MyPacket.MapIslands[i, j] != ID)
                    {

                        Islands[i, j].Click += EnemyIsland_Click;
                        Islands[i, j].Cursor = Cursors.Hand;
                        Islands[i, j].MouseMove += Island_MouseMove;
                        Islands[i, j].MouseLeave += Island_MouseLeave;

                    }
                }
            }
        }

        private void BeforeAttack()//攻擊前別人的島嶼不能點
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (MyPacket.MapIslands[i, j] != ID)
                    {
                        Islands[i, j].Enabled = false;
                    }
                    else
                    {
                        Islands[i, j].Enabled = true;
                    }

                    
                }
            }
        }

        private void BeforeCancel(int x,int y) //再次點擊已選擇島嶼前,不可以點擊任何其它自己的島嶼,再次點擊後,島嶼消失
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (i == x && j == y)  //如果是自己選擇的島,也可以按
                    {
                        continue;
                    }

                    if(MyPacket.MapIslands[i,j]!=ID)  //如果位於旁邊的島不是自己的,則可以按
                    {
                        if (i == x + 1 && j == y) { Islands[i, j].Enabled = true; }
                        else if (i == x - 1 && j == y) { Islands[i, j].Enabled = true; }
                        else if (i == x && j == y+1) { Islands[i, j].Enabled = true; }
                        else if (i == x && j == y-1) { Islands[i, j].Enabled = true; }

                       continue;
                    }
                    
                    
                        Islands[i, j].Enabled = false;  //其他通通不能按
                    
                }
            }
        
        }

        private void BeforeBattle(int x,int y) //除了該島其他通通不能按
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (i == x && j == y)  //如果是自己選擇的島,可以按
                    {
                        continue;
                    }

                    Islands[i, j].Enabled = false;  //其他通通不能按

                }
            }
        }

        private void IslandAttackAva(int AttackX,int AttackY)//可以攻擊的點位
        {
            if (AttackX - 1 >= 0 && MyPacket.MapIslands[AttackX-1, AttackY] != ID)
            {
                Islands[AttackX - 1, AttackY].Enabled = true;
            }
            if (AttackY - 1 >= 0 && MyPacket.MapIslands[AttackX, AttackY - 1] != ID)
            {
                Islands[AttackX, AttackY - 1].Enabled = true;
            }
            if (AttackX + 1 < 4 && MyPacket.MapIslands[AttackX + 1, AttackY] != ID)
            {
                Islands[AttackX + 1, AttackY].Enabled = true;
            }
            if (AttackY + 1 < 6 && MyPacket.MapIslands[AttackX, AttackY + 1] != ID)
            {
                Islands[AttackX, AttackY + 1].Enabled = true;
            }
        }

        private bool CheckWin()//判斷是否有輸贏
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (MyPacket.MapIslands[i, j] == ID)
                        continue;
                    else
                        return false;
                }
            }
            MyPacket.whoWin = ID;
            return true;
        }

        private void ReceiveAllInitialInfo_DoWork(object sender, DoWorkEventArgs e)//接收所有初始化資訊的BackgroundWorker
        {
            ReceiveIDThread();
            SendInfo();
            ReceiveAllInfoThread();
            ReceiveAllPacketThread();
        }

        private void ReceiveAllInitialInfo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)//遊戲開始畫面
        {
            GameStartView();
        }

        private void SndRcvPktInGame_DoWork(object sender, DoWorkEventArgs e)//接收遊戲過程中資訊的BackgroundWorker
        {                                    
            ReceiveAllPacketThread();                           
        }

        private void SndRcvPktInGame_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)//接收到Packet過後的遊戲畫面改變
        {
            if (MyPacket.NowID == ID)
            {
                GameProcessView(true);
            }
            else
            {
                GameProcessView(false);
            }
        }     


    }
}

namespace Multi_Threaded_Client
{
    partial class ClientForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.InputIPLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.ChooseCharacterLabel = new System.Windows.Forms.Label();
            this.LeaveButton = new System.Windows.Forms.Button();
            this.LoginButton = new System.Windows.Forms.Button();
            this.CharacterLeftPictureBox = new System.Windows.Forms.PictureBox();
            this.CharacterRightPictureBox = new System.Windows.Forms.PictureBox();
            this.CharacterPictureBox = new System.Windows.Forms.PictureBox();
            this.BackgroundPictureBox = new System.Windows.Forms.PictureBox();
            this.ReceiveAllInitialInfo = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.CharacterLeftPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterRightPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroundPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // IPTextBox
            // 
            this.IPTextBox.Font = new System.Drawing.Font("新細明體", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.IPTextBox.Location = new System.Drawing.Point(893, 82);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(264, 42);
            this.IPTextBox.TabIndex = 1;
            this.IPTextBox.Text = "127.0.0.1";
            this.IPTextBox.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // InputIPLabel
            // 
            this.InputIPLabel.AutoSize = true;
            this.InputIPLabel.Font = new System.Drawing.Font("微軟正黑體", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.InputIPLabel.ForeColor = System.Drawing.Color.Brown;
            this.InputIPLabel.Location = new System.Drawing.Point(732, 82);
            this.InputIPLabel.Name = "InputIPLabel";
            this.InputIPLabel.Size = new System.Drawing.Size(140, 37);
            this.InputIPLabel.TabIndex = 1;
            this.InputIPLabel.Text = "ServerIP:";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("微軟正黑體", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.NameLabel.ForeColor = System.Drawing.Color.Brown;
            this.NameLabel.Location = new System.Drawing.Point(790, 160);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(82, 37);
            this.NameLabel.TabIndex = 6;
            this.NameLabel.Text = "名稱:";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Font = new System.Drawing.Font("新細明體", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.NameTextBox.Location = new System.Drawing.Point(893, 160);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(264, 42);
            this.NameTextBox.TabIndex = 2;
            this.NameTextBox.Text = "國王";
            this.NameTextBox.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // ChooseCharacterLabel
            // 
            this.ChooseCharacterLabel.AutoSize = true;
            this.ChooseCharacterLabel.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ChooseCharacterLabel.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.ChooseCharacterLabel.Location = new System.Drawing.Point(189, 349);
            this.ChooseCharacterLabel.Name = "ChooseCharacterLabel";
            this.ChooseCharacterLabel.Size = new System.Drawing.Size(134, 31);
            this.ChooseCharacterLabel.TabIndex = 4;
            this.ChooseCharacterLabel.Text = "請選擇角色";
            // 
            // LeaveButton
            // 
            this.LeaveButton.BackColor = System.Drawing.Color.Gold;
            this.LeaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.LeaveButton.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.LeaveButton.Location = new System.Drawing.Point(1002, 593);
            this.LeaveButton.Name = "LeaveButton";
            this.LeaveButton.Size = new System.Drawing.Size(95, 56);
            this.LeaveButton.TabIndex = 4;
            this.LeaveButton.Text = "離開";
            this.LeaveButton.UseVisualStyleBackColor = false;
            this.LeaveButton.Click += new System.EventHandler(this.LeaveButton_Click);
            // 
            // LoginButton
            // 
            this.LoginButton.BackColor = System.Drawing.Color.Gold;
            this.LoginButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.LoginButton.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.LoginButton.Location = new System.Drawing.Point(848, 593);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(95, 56);
            this.LoginButton.TabIndex = 3;
            this.LoginButton.Text = "登入";
            this.LoginButton.UseVisualStyleBackColor = false;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // CharacterLeftPictureBox
            // 
            this.CharacterLeftPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CharacterLeftPictureBox.Image = global::Multi_Threaded_Client.Properties.Resources.LeftButton;
            this.CharacterLeftPictureBox.Location = new System.Drawing.Point(88, 488);
            this.CharacterLeftPictureBox.Name = "CharacterLeftPictureBox";
            this.CharacterLeftPictureBox.Size = new System.Drawing.Size(38, 81);
            this.CharacterLeftPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CharacterLeftPictureBox.TabIndex = 5;
            this.CharacterLeftPictureBox.TabStop = false;
            this.CharacterLeftPictureBox.Click += new System.EventHandler(this.CharacterLeftPictureBox_Click);
            // 
            // CharacterRightPictureBox
            // 
            this.CharacterRightPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CharacterRightPictureBox.Image = global::Multi_Threaded_Client.Properties.Resources.RightButton;
            this.CharacterRightPictureBox.Location = new System.Drawing.Point(390, 488);
            this.CharacterRightPictureBox.Name = "CharacterRightPictureBox";
            this.CharacterRightPictureBox.Size = new System.Drawing.Size(36, 81);
            this.CharacterRightPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CharacterRightPictureBox.TabIndex = 5;
            this.CharacterRightPictureBox.TabStop = false;
            this.CharacterRightPictureBox.Click += new System.EventHandler(this.CharacterRightPictureBox_Click);
            // 
            // CharacterPictureBox
            // 
            this.CharacterPictureBox.Image = global::Multi_Threaded_Client.Properties.Resources.國王;
            this.CharacterPictureBox.Location = new System.Drawing.Point(154, 402);
            this.CharacterPictureBox.Name = "CharacterPictureBox";
            this.CharacterPictureBox.Size = new System.Drawing.Size(210, 247);
            this.CharacterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CharacterPictureBox.TabIndex = 3;
            this.CharacterPictureBox.TabStop = false;
            // 
            // BackgroundPictureBox
            // 
            this.BackgroundPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackgroundPictureBox.Image = global::Multi_Threaded_Client.Properties.Resources.封面背景;
            this.BackgroundPictureBox.Location = new System.Drawing.Point(0, 0);
            this.BackgroundPictureBox.Name = "BackgroundPictureBox";
            this.BackgroundPictureBox.Size = new System.Drawing.Size(1213, 680);
            this.BackgroundPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BackgroundPictureBox.TabIndex = 0;
            this.BackgroundPictureBox.TabStop = false;
            // 
            // ReceiveAllInitialInfo
            // 
            this.ReceiveAllInitialInfo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ReceiveAllInitialInfo_DoWork);
            this.ReceiveAllInitialInfo.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ReceiveAllInitialInfo_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 7000;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1213, 680);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.LeaveButton);
            this.Controls.Add(this.CharacterLeftPictureBox);
            this.Controls.Add(this.CharacterRightPictureBox);
            this.Controls.Add(this.ChooseCharacterLabel);
            this.Controls.Add(this.CharacterPictureBox);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.InputIPLabel);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.IPTextBox);
            this.Controls.Add(this.BackgroundPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ClientForm";
            this.Text = "骰子戰爭";
            ((System.ComponentModel.ISupportInitialize)(this.CharacterLeftPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterRightPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroundPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox BackgroundPictureBox;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label InputIPLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.PictureBox CharacterPictureBox;
        private System.Windows.Forms.Label ChooseCharacterLabel;
        private System.Windows.Forms.PictureBox CharacterRightPictureBox;
        private System.Windows.Forms.PictureBox CharacterLeftPictureBox;
        private System.Windows.Forms.Button LeaveButton;
        private System.Windows.Forms.Button LoginButton;
        private System.ComponentModel.BackgroundWorker ReceiveAllInitialInfo;
        private System.Windows.Forms.Timer timer1;
    }
}


using MediCare.DataHandling;
using MediCare.NetworkLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediCare.ArtsClient
{
    public partial class DoctorClient : Form
    {
        private readonly System.Windows.Forms.Timer _getActiveClientsTimer;
        private readonly System.Windows.Forms.Timer _labelRemoveTimer;

        private static string _server = NetworkSettings.SERVERIP;
        private static int _port = NetworkSettings.SERVERPORT;
        private ClientTcpConnector _client;

        private string _connectedIDs = "";

        private string _ID;
        private Boolean _userIsAuthenticated = false;

        private Dictionary<string, clientTab> _tabIdDict = new Dictionary<string, clientTab>();
        private List<clientTab> _tabs = new List<clientTab>();
        private List<string> _ids = new List<string>();

        private SignupTool _signupTool;
        private ManageUsersTool _manageUsersTool;

        //USE THIS FORMAT WHEN SENDING DATETIME FOR fILEIO!
        private string DateFileFormat = "yyyy_MM_dd HH_mm_ss";

        public DoctorClient()
        {
            InitializeComponent();

            setVisibility(false);
            this.FormClosing += on_Window_Closed_Event;

            //opzetten tcp connectie
            TcpClient TcpClient = new TcpClient(_server, _port);
            _client = new ClientTcpConnector(TcpClient, _server);


            // haalt de de actieve clients op elke 1s
            _getActiveClientsTimer = new System.Windows.Forms.Timer();
            _getActiveClientsTimer.Interval = 1000;
            _getActiveClientsTimer.Tick += updateActiveClients;


            // timer voor het verwijderen van de errortekst na 3s, 'cosmetisch'
            _labelRemoveTimer = new System.Windows.Forms.Timer();
            _labelRemoveTimer.Interval = 3000;
            _labelRemoveTimer.Tick += UpdateLabel;

            // height van de table moet je handmatig setten.....
            OverviewTable.RowTemplate.MinimumHeight = 50;

            new Thread(() =>
            {
                while (true)
                {
                    if (_userIsAuthenticated)
                    {
                        Packet packet = null;
                        if (_client.isConnected() && !this.IsDisposed)
                        {
                            packet = _client.ReadMessage();
                            if (packet != null)
                            {
                                processPacket(packet);
                            }
                        }
                    }
                    Thread.Sleep(5);
                }
            }).Start();
        }

        private void processPacket(Packet p)
        {
            if (this.InvokeRequired && _client.isConnected())
            {
                try
                {
                    this.Invoke(new Action<Packet>(processPacket), new object[] { p });
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                switch (p._type)
                {
                    //sender = incoming client
                    //packet = data van de client
                    case "Chat":
                    HandleChatPacket(p);
                    break;
                    case "Data":
                    HandleDataPacket(p);
                    break;
                    case "ActiveClients":
                    HandleActiveClientsPacket(p);
                    break;
                    case "Filelist":
                    HandleFilelistPacket(p);
                    break;
                    case "FileRequest":
                    HandleFileRequest(p);
                    break;
                    case "Disconnect":
                    HandleDisconnectPacket(p);
                    break;
                    default: //nothing
                    break;
                }
            }
        }

        private void HandleDisconnectPacket(Packet p)
        {
            _client.Close();
        }
        private void HandleFileRequest(Packet p)
        {
            throw new NotImplementedException();
            String[] files = p._message.Split('-');
            foreach (string file in files)
            {
                MessageBox.Show(file);
                Packet request = new Packet("98765432", "FileRequest", "server", "12345678-" + file);
                _client.sendMessage(request);
            }
        }
        private void HandleChatPacket(Packet p)
        {
            on_message_receive_event(p._id, p._message);
        }

        private void HandleDataPacket(Packet p)
        {
            string[] data = p._message.Split(' ');
            //Console.WriteLine("MESSAGE: " + p._message);
            if (_tabIdDict.ContainsKey(p._id))
            {
                _tabIdDict[p._id].UpdateValues(data);
            }
        }

        private void HandleActiveClientsPacket(Packet p)
        {
            _connectedIDs = p.GetMessage();
            string[] ids = getActiveClients().Split(' ');
            //Console.WriteLine("Active Clients: " + connectedIDs);

            int rowNumber = 1;
            foreach (string id in ids)
            {
                if (!_ids.Contains(id) && id != "")
                {
                    this.OverviewTable.Rows.Add(id);
                    _ids.Add(id);
                }
            }
            foreach (DataGridViewRow row in OverviewTable.Rows)
            {
                if (row.IsNewRow)
                    continue;
                row.HeaderCell.Value = "Client no. " + rowNumber;
                rowNumber = rowNumber + 1;
            }

        }

        private void HandleFilelistPacket(Packet p)
        {
            Filelist.Items.Clear();
            string[] files = p._message.Split('-');
            foreach (string file in files)
            {
                Filelist.Items.Add(file);
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string id = (string)OverviewTable.CurrentCell.Value;
            if (!tabControl1.Controls.ContainsKey(id))
            {
                if (_client.isConnected())
                {
                    clientTab tab = new clientTab(id, _client, _ID);
                    if (!_tabs.Contains(tab))
                        _tabs.Add(tab);
                    if (!_tabIdDict.ContainsKey(id))
                        _tabIdDict.Add(id, tab);

                    tab.closeAllButThisButton.Click += new System.EventHandler(On_Tab_Close_All_Event);
                    tab.closeButton.Click += new System.EventHandler(On_Tab_Closed_Event);
                    this.tabControl1.Controls.Add(tab);
                    this.tabControl1.SelectedTab = tab;
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            string patientID = (string)OverviewTable.CurrentCell.Value;
            Packet p = new Packet(_ID, "Filelist", "server", patientID);
            Console.WriteLine("Patient id : " + patientID);
            _client.sendMessage(p);
        }

        /**
         * When a client connects start a thread with that client (not here)
         * 
         * What should happen here is a new entry in the client list should be made so that the clientContainer (see design)
         * updates the screen with the new connected client.
         */
        private void on_client_connect_event() // "Client client" Iets in die trend.
        {

        }

        private void updateActiveClients(object sender, EventArgs e)
        {
            if(_userIsAuthenticated)
            {
                Packet response = new Packet(_ID, "ActiveClients", "Server", "Get active clients");
                _client.sendMessage(response);
            }
        }

        private string getActiveClients()
        {
            return _connectedIDs;
        }

        // code wordt niet gebruikt?
        private void getClients()
        {
            string[] ids = getActiveClients().Split(' ');
            for (int i = 0; i < ids.Length; i++)
            {
                if (!this.tabControl1.Controls.ContainsKey(ids[i]))
                {
                    clientTab tab = new clientTab(ids[i], _client, _ID);
                    this.tabControl1.Controls.Add(tab);
                }
            }
        }

        # region TabControl Event Handlers
        private void On_Tab_Closed_Event(Object Sender, EventArgs e)
        {
            /* 'Clean' code als een client disocnnect
            _ids.Remove(this.tabControl1.SelectedTab.Name);
            _tabIdDict.Remove(this.tabControl1.SelectedTab.Name);
             this.dataGridView1.Rows.RemoveAt(this.dataGridView1.CurrentCell.RowIndex);
           */
            _tabIdDict.Remove(this.tabControl1.SelectedTab.Name);
            this.tabControl1.Controls.RemoveAt(this.tabControl1.SelectedIndex);
        }

        //Skip zero because that is our main screen. Also dont close the Current Tab
        private void On_Tab_Close_All_Event(Object Sender, EventArgs e)
        {
            foreach (System.Windows.Forms.TabPage tab in this.tabControl1.TabPages)
            {
                if (!tab.Name.Equals("IndexTab"))
                {
                    this.tabControl1.Controls.Remove(tab);
                }
            }
        }
        # endregion

        # region Chat Box
        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            //nonedonexD
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (typeBox.Text != "")
            {
                txtLog.AppendText(Environment.NewLine + "Me: " + typeBox.Text);
                typeBox.Text = "";

            }
        }

        private void txtLog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (typeBox.Text != "")
                {
                    foreach (string key in _tabIdDict.Keys)
                    {
                        Packet p = new Packet(_ID + " [Broadcast]", "Chat", key, typeBox.Text);
                        _client.sendMessage(p);
                        _tabIdDict[key].UpdateChatBox("Me [Broadcast]", typeBox.Text);
                    }
                    txtLog.AppendText(Environment.NewLine + "Me [Broadcast]: " + typeBox.Text);
                    txtLog_AlignTextToBottom();
                    txtLog_ScrollToBottom();
                    typeBox.Text = "";
                }
            }
        }

        private void txtLog_AlignTextToBottom()
        {
            int visibleLines = (int)(txtLog.Height / txtLog.Font.GetHeight()) - 50;
            if (visibleLines > txtLog.Lines.Length)
            {
                int emptyLines = (visibleLines - txtLog.Lines.Length);
                for (int i = 0; i < emptyLines; i++)
                {
                    txtLog.Text = (Environment.NewLine + txtLog.Text);
                }
            }
        }

        private void txtLog_ScrollToBottom()
        {
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        delegate void UpdateChat(string identification, string text);
        public void on_message_receive_event(string id, string message)
        {
            if (this.txtLog.InvokeRequired)
            {
                UpdateChat d = new UpdateChat(on_message_receive_event);
                this.Invoke(d, new object[] { id, message });
            }
            else
            {
                txtLog.AppendText(Environment.NewLine + id + ": " + message);
                typeBox.Text = "";
                txtLog_AlignTextToBottom();
                txtLog_ScrollToBottom();
            }
            if (_tabIdDict.ContainsKey(id))
            {
                _tabIdDict[id].UpdateChatBox(id, message);
            }
        }

        # endregion

        #region TCPclient tools
        //Depricated read and send methods should not be used.. use the ClientTcpConnector instead
        # endregion

        #region login

        private void setVisibility(bool v)
        {
            //TODO: table niet of wel visible maken
            IndexTab.Visible = v;
            OverviewLabel.Visible = v;
            tabControl1.Visible = v;
            SendMessage.Visible = v;
            typeBox.Visible = v;
            txtLog.Visible = v;
            Password_Box.Visible = v;
            Username_Box.Visible = v;
            Password_Label.Visible = v;
            Username_label.Visible = v;
            LoginButton.Visible = v;

            Password_Box.Visible = !v;
            Username_Box.Visible = !v;
            Password_Label.Visible = !v;
            Username_label.Visible = !v;
            LoginButton.Visible = !v;
            Error_Label.Visible = !v;
            if (!v)
            {
                this.ActiveControl = Username_Box;
            }
            else
            {
                this.ActiveControl = typeBox;
            }
        }

        private void on_username_box_enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Username_Box.Focused)
            {
                this.ActiveControl = Password_Box;
            }
        }

        private void on_password_box_enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Password_Box.Focused)
            {
                login(sender, e);
            }
        }

        private void login(object sender, EventArgs e)
        {
            //Login to server bla bla bla
            Regex r = new Regex(@"^[0-9]{8}$");
            if (String.IsNullOrEmpty(Username_Box.Text) || String.IsNullOrEmpty(Password_Box.Text))
            {
                Error_Label.Text = "One or more fields are blank!";
                _labelRemoveTimer.Start();
                this.ActiveControl = Username_Box;
            }
            else if (!Username_Box.Text.StartsWith("9") || (!r.IsMatch(Username_Box.Text)))
            {
                Error_Label.Text = "Doctor ID must start with a 9 and is 8 digits long!";
                _labelRemoveTimer.Start();
                this.ActiveControl = Username_Box;
            }
            else
            {
                string tempID = Username_Box.Text;
                
                _client.sendFirstConnectPacket(tempID, Password_Box.Text);

                while (!_userIsAuthenticated)
                {
                    //pol for packets. if packet == authenticated!

                    Packet packet = null;
                    if (_client.isConnected())
                    {
                        packet = _client.ReadMessage();

                        if (packet._message.Equals("VERIFIED"))
                        {
                            //todo check for authenticated packet from server 
                            _userIsAuthenticated = true;
                            _ID = tempID;

                            setVisibility(true);
                            _getActiveClientsTimer.Start(); // automatisch ophalen van de actieve verbindingen

                            _signupTool = new SignupTool(_ID);
                            _signupTool.Hide();

                            _manageUsersTool = new ManageUsersTool(_ID);
                            _manageUsersTool.Hide();
                        }
                        else
                        {
                            displayErrorMessage(packet._message);
                            break;
                        }
                    }
                }
            }
            //{
            //    _ID = Username_Box.Text;
            //    setVisibility(true);
            //    getActiveClientsTimer.Start(); // automatisch ophalen van de actieve verbindingen      
            //}
        }

        private void displayErrorMessage(string message)
        {
            Error_Label.Text = message;
            _labelRemoveTimer.Start();
        }

        private void UpdateLabel(object sender, EventArgs e)
        {
            Error_Label.Text = "";
            _labelRemoveTimer.Stop();
        }
        #endregion

        private void on_Window_Closed_Event(object sender, FormClosingEventArgs e)
        {
            Packet p = new Packet(_ID, "Disconnect", "Server", "Disconnecting");
            //send message to server that ur dying
            if (_client.isConnected())
            {
                _client.sendMessage(p);
            }
        }
        private void ManageUsersButton_Click(object sender, EventArgs e)
        {
            if (_signupTool.Visible)
                _manageUsersTool.Hide();
            else
                _manageUsersTool.Show();
        }

        private void Signup_Button_Click(object sender, EventArgs e)
        {
            if (_signupTool.Visible)
                _signupTool.Hide();
            else
                _signupTool.Show();
        }

        private void Filelist_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.Filelist.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {

                MessageBox.Show(Filelist.Items[index].ToString());
            }
        }
    }

    #region Tab generation

    public class clientTab : TabPage
    {
        public Graph graph;
        #region define Controls
        private System.Windows.Forms.Button TestStartButton;
        private System.Windows.Forms.Button TestResultsButton;
        public Button closeButton = new Button();
        public Button closeAllButThisButton = new Button();
        private TextBox chatBox = new TextBox();
        private TextBox typeBox = new TextBox();
        private Button sendButtonClient = new Button();

        private Button updatePowerButton = new Button();
        private Button resetButton = new Button();
        public TextBox newPowerBox = new TextBox();
        private Label newPowerLabel = new Label();
        private Label RPMLabel = new Label();
        private TextBox Speed_Box = new TextBox();
        private TextBox Distance_Box = new TextBox();
        private TextBox Brake_Box = new TextBox();
        private TextBox Power_Box = new TextBox();
        private TextBox Energy_Box = new TextBox();
        private TextBox Heartbeats_Box = new TextBox();
        private TextBox RPM_Box = new TextBox();
        private Label speedLabel = new Label();
        private Label brakeLabel = new Label();
        private Label powerLabel = new Label();
        private Label heartBeatsLabel = new Label();
        private Label energyLabel = new Label();
        private Label distanceLabel = new Label();
        private TextBox TimeRunning_Box = new TextBox();
        private Label timeRunningLabel = new Label();
        #endregion

        private ClientTcpConnector _client;
        private string _tabName;
        private string _id;

        public clientTab(string tabName, ClientTcpConnector client, string id) //loads of data etc... (joke)
        {
            this._client = client;
            this._id = id;
            this._tabName = tabName;

            graph = new Graph();
            graph.Initialize_Checkboxes_Doctor();
            graph.InitializeChart_Doctor();
            graph.InitializeGraph();
            AddGraphToForm();

            #region Close Buttons
            //close button
            closeButton.Location = new System.Drawing.Point(1070, 600);
            closeButton.Text = "Close";

            //close all but this button
            closeAllButThisButton.Location = new System.Drawing.Point(1150, 600);
            closeAllButThisButton.Text = "Close all";
            #endregion

            #region chatbox
            //ChatBox
            chatBox.AllowDrop = true;
            chatBox.BackColor = System.Drawing.Color.WhiteSmoke;
            chatBox.Location = new System.Drawing.Point(20, 390);
            chatBox.Multiline = true;
            chatBox.Name = "txtLog";
            chatBox.ReadOnly = true;
            chatBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            chatBox.Size = new System.Drawing.Size(980, 210);
            chatBox.TabIndex = 6;
            chatBox.TextChanged += new System.EventHandler(this.txtLog_TextChanged);

            //TypeBox
            typeBox.Location = new System.Drawing.Point(20, 603);
            typeBox.Name = "typeBox";
            typeBox.Size = new System.Drawing.Size(902, 20);
            typeBox.TabIndex = 9;
            typeBox.KeyDown += new System.Windows.Forms.KeyEventHandler(txtLog_KeyDown);

            //sendButton
            sendButtonClient.Location = new System.Drawing.Point(928, 600);
            sendButtonClient.Name = "SendMessage";
            sendButtonClient.Size = new System.Drawing.Size(75, 23);
            sendButtonClient.TabIndex = 10;
            sendButtonClient.Text = "Send";
            sendButtonClient.UseVisualStyleBackColor = true;
            sendButtonClient.Click += new System.EventHandler(this.sendButton_Click);
            #endregion

            #region update Power Button
            // 
            // updatePowerButton
            // 
            updatePowerButton.Location = new System.Drawing.Point(760, 437);
            updatePowerButton.Name = "updatePowerButton";
            updatePowerButton.Size = new System.Drawing.Size(63, 35);
            updatePowerButton.TabIndex = 38;
            updatePowerButton.Text = "Update";
            updatePowerButton.UseVisualStyleBackColor = true;
            #endregion

            #region Reset Button
            // 
            // Reset Button
            // 
            resetButton.Location = new System.Drawing.Point(20, 350);
            resetButton.Name = "resetButton";
            resetButton.Size = new System.Drawing.Size(70, 35);
            resetButton.TabIndex = 39;
            resetButton.Text = "Reset";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += new System.EventHandler(this.resetButton_Click);
            #endregion

            #region TimeRunning
            // 
            // TIME
            // 
            TimeRunning_Box.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TimeRunning_Box.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            TimeRunning_Box.Location = new System.Drawing.Point(220, 20);
            TimeRunning_Box.Name = "TimeRunning_Box";
            TimeRunning_Box.ReadOnly = true;
            TimeRunning_Box.Size = new System.Drawing.Size(200, 30);
            TimeRunning_Box.TabIndex = 21;
            TimeRunning_Box.TabStop = false;

            timeRunningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            timeRunningLabel.Location = new System.Drawing.Point(20, 20);
            timeRunningLabel.Name = "label1";
            timeRunningLabel.Size = new System.Drawing.Size(200, 30);
            timeRunningLabel.TabIndex = 20;
            timeRunningLabel.Text = "Time Running";
            #endregion

            #region speed
            // 
            // SPEED
            // 
            speedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            speedLabel.Location = new System.Drawing.Point(20, 55);
            speedLabel.Name = "label7";
            speedLabel.Size = new System.Drawing.Size(200, 30);
            speedLabel.TabIndex = 27;
            speedLabel.Text = "Speed";

            Speed_Box.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Speed_Box.Location = new System.Drawing.Point(220, 55);
            Speed_Box.Name = "Speed_Box";
            Speed_Box.ReadOnly = true;
            Speed_Box.Size = new System.Drawing.Size(200, 30);
            Speed_Box.TabIndex = 34;
            #endregion

            #region Distance
            // 
            // DISTANCE
            // 
            distanceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            distanceLabel.Location = new System.Drawing.Point(20, 90);
            distanceLabel.Name = "label2";
            distanceLabel.Size = new System.Drawing.Size(200, 30);
            distanceLabel.TabIndex = 22;
            distanceLabel.Text = "Distance";

            Distance_Box.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Distance_Box.Location = new System.Drawing.Point(220, 90);
            Distance_Box.Name = "Distance_Box";
            Distance_Box.ReadOnly = true;
            Distance_Box.Size = new System.Drawing.Size(200, 30);
            Distance_Box.TabIndex = 33;
            #endregion

            #region Brake
            //
            // BRAKE
            //
            brakeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            brakeLabel.Location = new System.Drawing.Point(20, 125);
            brakeLabel.Name = "label6";
            brakeLabel.Size = new System.Drawing.Size(200, 30);
            brakeLabel.TabIndex = 26;
            brakeLabel.Text = "Brake";

            Brake_Box.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Brake_Box.Location = new System.Drawing.Point(220, 125);
            Brake_Box.Name = "Brake_Box";
            Brake_Box.ReadOnly = true;
            Brake_Box.Size = new System.Drawing.Size(200, 30);
            Brake_Box.TabIndex = 32;
            #endregion

            #region Power
            // 
            // POWER
            // 


            Power_Box.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Power_Box.Location = new System.Drawing.Point(220, 160);
            Power_Box.Name = "Power_Box";
            Power_Box.ReadOnly = true;
            Power_Box.Size = new System.Drawing.Size(200, 30);
            Power_Box.TabIndex = 31;

            powerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            powerLabel.Location = new System.Drawing.Point(20, 160);
            powerLabel.Name = "label5";
            powerLabel.Size = new System.Drawing.Size(200, 30);
            powerLabel.TabIndex = 25;
            powerLabel.Text = "Power";
            #endregion

            #region Energy
            //
            //   ENERGY
            //
            Energy_Box.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Energy_Box.Location = new System.Drawing.Point(220, 195);
            Energy_Box.Name = "Energy_Box";
            Energy_Box.ReadOnly = true;
            Energy_Box.Size = new System.Drawing.Size(200, 30);
            Energy_Box.TabIndex = 30;

            energyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            energyLabel.Location = new System.Drawing.Point(20, 195);
            energyLabel.Name = "label3";
            energyLabel.Size = new System.Drawing.Size(200, 30);
            energyLabel.TabIndex = 23;
            energyLabel.Text = "Energy";
            #endregion

            #region Inspanningstestbuttons
            this.TestStartButton = new System.Windows.Forms.Button();
            this.TestResultsButton = new System.Windows.Forms.Button();
            // 
            // TestStartButton
            // 
            this.TestStartButton.Location = new System.Drawing.Point(1020, 420);
            this.TestStartButton.Name = "TestStartButton";
            this.TestStartButton.Size = new System.Drawing.Size(120, 45);
            this.TestStartButton.TabIndex = 36;
            this.TestStartButton.Text = "Start Inspanningstest";
            this.TestStartButton.UseVisualStyleBackColor = true;
            this.TestStartButton.Click += new System.EventHandler(this.TestStartButton_Clicked);
            // 
            // TestResultsButton
            // 
            this.TestResultsButton.Location = new System.Drawing.Point(1020, 480);
            this.TestResultsButton.Name = "TestResultsButton";
            this.TestResultsButton.Size = new System.Drawing.Size(120, 45);
            this.TestResultsButton.TabIndex = 37;
            this.TestResultsButton.Text = "Resultaten Inspanningstest";
            this.TestResultsButton.UseVisualStyleBackColor = true;
            this.TestResultsButton.Click += new System.EventHandler(this.TestResultsButton_Clicked);

            #endregion

            #region Heart Beats
            // 
            // HEARTBEATS
            // 
            heartBeatsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            heartBeatsLabel.Location = new System.Drawing.Point(20, 230);
            heartBeatsLabel.Name = "label4";
            heartBeatsLabel.Size = new System.Drawing.Size(200, 30);
            heartBeatsLabel.TabIndex = 24;
            heartBeatsLabel.Text = "Heartbeats";

            Heartbeats_Box.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Heartbeats_Box.Location = new System.Drawing.Point(220, 230);
            Heartbeats_Box.Name = "Heartbeats_Box";
            Heartbeats_Box.ReadOnly = true;
            Heartbeats_Box.Size = new System.Drawing.Size(200, 30);
            Heartbeats_Box.TabIndex = 29;
            #endregion

            #region RPM
            //
            // RPM
            //
            RPM_Box.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            RPM_Box.Location = new System.Drawing.Point(220, 265);
            RPM_Box.Name = "RPM_Box";
            RPM_Box.ReadOnly = true;
            RPM_Box.Size = new System.Drawing.Size(200, 30);
            RPM_Box.TabIndex = 28;

            RPMLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            RPMLabel.Location = new System.Drawing.Point(20, 265);
            RPMLabel.Name = "label8";
            RPMLabel.Size = new System.Drawing.Size(200, 30);
            RPMLabel.TabIndex = 35;
            RPMLabel.Text = "RPM";
            #endregion

            #region New Powerbox
            // 
            // NEW POERRBOX
            // 
            newPowerBox.AcceptsReturn = true;
            newPowerBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            newPowerBox.Location = new System.Drawing.Point(220, 310);
            newPowerBox.Name = "newPowerBox";
            newPowerBox.Size = new System.Drawing.Size(200, 30);
            newPowerBox.TabIndex = 37;
            newPowerBox.Text = "Enter new value";
            newPowerBox.GotFocus += new System.EventHandler(this.newPowerBox_GotFocus);
            newPowerBox.Leave += new System.EventHandler(this.newPowerBox_Leave);
            newPowerBox.KeyDown += new System.Windows.Forms.KeyEventHandler(newPowerBox_KeyDown);

            newPowerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            newPowerLabel.Location = new System.Drawing.Point(20, 310);
            newPowerLabel.Name = "label9";
            newPowerLabel.Size = new System.Drawing.Size(200, 30);
            newPowerLabel.TabIndex = 36;
            newPowerLabel.Text = "New power";
            #endregion


            #region add components
            //add components
            this.Controls.Add(closeButton);
            this.Controls.Add(closeAllButThisButton);
            this.Controls.Add(chatBox);
            this.Controls.Add(typeBox);
            this.Controls.Add(sendButtonClient);
            this.Controls.Add(updatePowerButton);
            this.Controls.Add(newPowerBox);
            this.Controls.Add(newPowerLabel);
            this.Controls.Add(resetButton);
            this.Controls.Add(RPMLabel);
            this.Controls.Add(Speed_Box);
            this.Controls.Add(Distance_Box);
            this.Controls.Add(Brake_Box);
            this.Controls.Add(Power_Box);
            this.Controls.Add(Energy_Box);
            this.Controls.Add(Heartbeats_Box);
            this.Controls.Add(RPM_Box);
            this.Controls.Add(speedLabel);
            this.Controls.Add(brakeLabel);
            this.Controls.Add(powerLabel);
            this.Controls.Add(heartBeatsLabel);
            this.Controls.Add(energyLabel);
            this.Controls.Add(distanceLabel);
            this.Controls.Add(TimeRunning_Box);
            this.Controls.Add(timeRunningLabel);
            this.Controls.Add(TestStartButton);
            this.Controls.Add(TestResultsButton);
            #endregion

            //set tab Settings
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Location = new System.Drawing.Point(4, 22);
            this.Name = tabName;
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(1232, 631);
            //this.TabIndex = this.tabControl1.TabCount + 1; NOT NEEDED???
            this.Text = tabName;
            this.UseVisualStyleBackColor = true;           
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset the bike?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Packet p = new Packet(_id, "Command", _tabName, "reset");
                _client.sendMessage(p);
                MessageBox.Show("Bike reset!");
            }
        }

        delegate void UpdateValueCallback(string[] text);

        public void UpdateValues(string[] data)
        {
            if (data.Length < 8)
            {
                Heartbeats_Box.Text = "0";
                RPM_Box.Text = "0";
                Speed_Box.Text = "0";
                Distance_Box.Text = "0";
                Power_Box.Text = "0";
                Energy_Box.Text = "0";
                TimeRunning_Box.Text = "0";
                Brake_Box.Text = "0";
            }
            else
            {
                if (this.Heartbeats_Box.InvokeRequired && this.RPM_Box.InvokeRequired && this.Speed_Box.InvokeRequired && this.Distance_Box.InvokeRequired &&
                     this.Power_Box.InvokeRequired && this.Energy_Box.InvokeRequired && this.TimeRunning_Box.InvokeRequired && this.Brake_Box.InvokeRequired)
                {
                    UpdateValueCallback d = new UpdateValueCallback(UpdateValues);
                    this.Invoke(d, new object[] { data });
                }
                else
                {
                    Heartbeats_Box.Text = data[0];
                    RPM_Box.Text = data[1];
                    Speed_Box.Text = data[2];
                    Distance_Box.Text = data[3];
                    Power_Box.Text = data[4];
                    Energy_Box.Text = data[5];
                    TimeRunning_Box.Text = data[6];
                    Brake_Box.Text = data[7];
                    graph.process_Graph_Data(data);

                }
            }
        }

        # region Chat Box
        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            //nonedonexD
        }

        delegate void UpdateTextChatBox(string id, string text);
        public void UpdateChatBox(string id, string message)
        {
            if (this.chatBox.InvokeRequired)
            {
                UpdateTextChatBox d = new UpdateTextChatBox(UpdateChatBox);
                this.Invoke(d, new object[] { id, message });
            }
            else
            {
                chatBox.AppendText(Environment.NewLine + id + ": " + message);
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (typeBox.Text != "")
            {
                chatBox.AppendText(Environment.NewLine + "Me: " + typeBox.Text);
                typeBox.Text = "";
            }
        }

        private void txtLog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (typeBox.Text != "")
                {
                    Packet p = new Packet(_id, "Chat", _tabName, typeBox.Text);
                    _client.sendMessage(p);
                    //txtLog.AppendText("");
                    chatBox.AppendText(Environment.NewLine + "Me: " + typeBox.Text);
                    chatBox_AlignTextToBottom();
                    chatBox_ScrollToBottom();
                    typeBox.Text = "";
                }
            }
        }

        private void chatBox_AlignTextToBottom()
        {
            int visibleLines = (int)(chatBox.Height / chatBox.Font.GetHeight()) - 50;
            if (visibleLines > chatBox.Lines.Length)
            {
                int emptyLines = (visibleLines - chatBox.Lines.Length);
                for (int i = 0; i < emptyLines; i++)
                {
                    chatBox.Text = (Environment.NewLine + chatBox.Text);
                }
            }
        }

        private void chatBox_ScrollToBottom()
        {
            chatBox.SelectionStart = chatBox.Text.Length;
            chatBox.ScrollToCaret();
        }

        # endregion

        # region Power Box Tools
        // onderstaande twee methodes zijn voor het weergeven van de placeholder tekst
        private void newPowerBox_GotFocus(object sender, EventArgs e)
        {
            if (newPowerBox.Text == "Enter new value")
                newPowerBox.Text = "";
        }

        private void newPowerBox_Leave(object sender, EventArgs e)
        {
            if (newPowerBox.Text == "")
            {
                newPowerBox.Text = "Enter new value";
            }
            else
            {
                newPowerBox.Text = newPowerBox.Text;
            }
        }

        // 'Actionlistener' voor de new power textbox
        private void newPowerBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (newPowerBox.Text != "")
                {
                    Packet p = new Packet(_id, "Command", _tabName, newPowerBox.Text);
                    _client.sendMessage(p);
                    newPowerBox.Text = "";
                }
            }
        }

        # endregion

        #region Graph Datahandlers and EventListeners

        private void AddGraphToForm()
        {
            object[] data = graph.getComponents();
            Controls.Add((System.Windows.Forms.DataVisualization.Charting.Chart)data[0]);
            for (int i = 1; i < data.Length; i++)
            {
                this.Controls.Add((System.Windows.Forms.CheckBox)data[i]);

            }
        }
        #endregion
        private void TestStartButton_Clicked(object sender, EventArgs e)
        {
            Packet p = new Packet("_ID", "TestStart", _tabName, "Start test");
            _client.sendMessage(p);
            //Start Inspanningstest
        }

        private void TestResultsButton_Clicked(object sender, EventArgs e)
        {
            Packet p = new Packet("_ID", "TestResults", "Server", _tabName);
            _client.sendMessage(p);
        }
    }
    #endregion


}

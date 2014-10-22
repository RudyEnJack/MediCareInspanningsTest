using MediCare.DataHandling;
using MediCare.NetworkLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediCare
{
    public partial class SignupTool : Form
    {
        private readonly Timer _labelRemoveTimer;
        private static string _server = NetworkSettings.SERVERIP;
        private static int _port = NetworkSettings.SERVERPORT;
        private ClientTcpConnector _client;
        private string _id;

        public SignupTool(string id)
        {
            InitializeComponent();

            //verbinden met de server om registratie af te handelen
            TcpClient TcpClient = new TcpClient(_server, _port);
            _client = new ClientTcpConnector(TcpClient, _server);

            this._id = id;

            _labelRemoveTimer = new Timer();
            _labelRemoveTimer.Interval = 3000;
            _labelRemoveTimer.Tick += UpdateLabel;

            this.FormClosing += Form_FormClosing;

            _client.sendFirstConnectPacket(id + "r", "nopassword");
            Console.WriteLine(_client.ReadMessage()._message);
        }

        #region Key events

        private void on_username_box_enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Username_TextBox.Focused)
            {
                this.ActiveControl = Password_TextBox;
            }
        }
        private void on_password_box_enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Password_TextBox.Focused)
            {
                this.ActiveControl = Password_Verify_TextBox;
            }
        }
        private void on_verify_password_box_enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Password_Verify_TextBox.Focused)
            {
                login(sender, e); // pass it on to the follow up event
            }
        }
        #endregion

        #region Register stuff
        // use Secure Socket to send user registration data to server
        /**
         * TODO: Logging in for real on the server
         */
        private void login(object sender, EventArgs e)
        {
            string name = Username_TextBox.Text;
            string pass = Password_TextBox.Text;
            string verifypass = Password_Verify_TextBox.Text;

            _client.sendMessage(new Packet(_id + "r", "Registration", "Server", name + ":" + pass));
            string response = _client.ReadMessage().GetMessage();

            Regex r = new Regex("^[0-9]{8}$");

            if (response.Equals("INVALID_PASS"))
            {
                DisplayError("Password contains invalid characters!");
                this.ActiveControl = Password_TextBox;
            }
            //sender > enqueue
            else if (name.Equals("") || pass.Equals("") || verifypass.Equals(""))
            {
                DisplayError("One or more fields are blank!");
                this.ActiveControl = Username_TextBox;
            }
            else if (!pass.Equals(verifypass))
            {
                DisplayError("Passwords do not match!");
                this.ActiveControl = Password_TextBox;
            }
            else if (response.Equals("REGISTER_FAIL"))
            {
                DisplayError("User aleady exists!");
                this.ActiveControl = Username_TextBox;
            }
            else if ((!r.IsMatch(name) || Int32.Parse(name.Substring(0, 1)) == 9))
            {
                DisplayError("Username must start with 1-8 \n and are 8 characters long");
                this.ActiveControl = Username_TextBox;
            }
            else if (name != "" && pass != "" && pass.Equals(verifypass) && (response.Equals("REGISTER_SUCCESS")))
            {
                MessageBox.Show("Registered user " + name + " and saved!");
                this.Hide();
            }
        }


        #endregion
        public void endConnection()
        {
            this.Close();
        }
        private void DisplayError(string message)
        {
            Error_Label.Text = message;
            _labelRemoveTimer.Start();
        }
        private void UpdateLabel(object sender, EventArgs e)
        {
            Error_Label.Text = "";
            _labelRemoveTimer.Stop();
        }

        //  hide de window ipv het echt te sluiten
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }

}

using MediCare.NetworkLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediCare
{
    public partial class ManageUsersTool : Form
    {
        private readonly Timer _labelRemoveTimer;
        private static string _server = NetworkSettings.SERVERIP;
        private static int _port = NetworkSettings.SERVERPORT;
        private ClientTcpConnector _client;
        private string _id;
        private string _prevCellValue;

        public ManageUsersTool(string id)
        {
            InitializeComponent();

            //verbinden met de server om registratie af te handelen
            TcpClient TcpClient = new TcpClient(_server, _port);
            _client = new ClientTcpConnector(TcpClient, _server);

            this._id = id;

            _labelRemoveTimer = new Timer();
            _labelRemoveTimer.Interval = 3000;
            _labelRemoveTimer.Tick += UpdateLabel;

            this.FormClosing += ManageUsersTool_FormClosing;

            this.dataGridView1.EditingControlShowing += this.dataGridView1_EditingControlShowing;
            this.dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            this.dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

            this.dataGridView1.CellLeave += dataGridView1_CellLeave;
            this.dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;

            _client.sendFirstConnectPacket(id + "m", "nopassword");
            Console.WriteLine(_client.ReadMessage()._message);

            LoadUsers(false);
        }

        // alle users inladen
        private void LoadUsers(bool isRefresh)
        {
            if (isRefresh)
                this.dataGridView1.Rows.Clear();

            _client.sendMessage(new Packet(_id + "m", "ManageUsers", "Server", "GetLogins"));
            string[] response = _client.ReadMessage().GetMessage().Split('@');
            string[] ids = response[0].Split(' ');
            string[] pass = response[1].Split(' ');

            for (int i = 0; i < ids.Length; i++)
            {
                if (!string.IsNullOrEmpty(ids[i]))
                    this.dataGridView1.Rows.Add(ids[i], pass[i]);
            }

            int rowNumber = 1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;
                row.HeaderCell.Value = "Client no. " + rowNumber;
                rowNumber = rowNumber + 1;
            }
        }

        // event handlers voor delete buttons
        private void DeleteUserButton_Click(object sender, System.EventArgs e)
        {
            string loginsToDelete = string.Empty;
            string displayIDs = string.Empty;
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                loginsToDelete += dataGridView1[0, cell.RowIndex].Value + ":" + dataGridView1[1, cell.RowIndex].Value + "@";
                displayIDs += dataGridView1[0, cell.RowIndex].Value + ", ";
            }
            //Deleteuser@id:pass@id:pass@id:pass@...
            displayIDs = displayIDs.Trim().Remove(displayIDs.Length - 2);
            var confirmResult = MessageBox.Show("Do you really want to remove the client(s) " + displayIDs + "?", "Remove client", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.Yes)
            {
                _client.sendMessage(new Packet(_id + "m", "ManageUsers", "Server", "DeleteUser@" + loginsToDelete.Remove(loginsToDelete.Length - 1)));
                if (_client.ReadMessage()._message.Equals("User deleted"))
                    MessageBox.Show("Client(s) " + displayIDs + " removed!");
                //DisplayLabelMessage("Client(s) " + ids + " deleted!"); // message via label ipv popup window
                LoadUsers(true);
            }
        }

        private void DeleteAllUsersButton_Click(object sender, System.EventArgs e)
        {
            string loginsToDelete = string.Empty;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                loginsToDelete += row.Cells[0].Value + ":" + row.Cells[1].Value + "@";
            }
            var confirmResult = MessageBox.Show("Do you really want to remove all clients?", "Remove all clients", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.Yes)
            {
                _client.sendMessage(new Packet(_id + "m", "ManageUsers", "Server", "DeleteUser@" + loginsToDelete.Remove(loginsToDelete.Length - 1)));
                if (_client.ReadMessage()._message.Equals("User deleted"))
                    MessageBox.Show("All users removed!");
                //DisplayLabelMessage("Client(s) " + ids + " deleted!"); // message via label ipv popup window
                LoadUsers(true);
            }
        }


        // sturen naar de server als je op enter drukt
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                string value = (string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!string.IsNullOrEmpty(value) && !_prevCellValue.Equals(value))
                {
                    string id = (string)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                    string newpass = (string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                    bool doesthepasswordcontainweirdcharsthatyoudontwantinapassword = Regex.IsMatch(newpass, @"[^A-Za-z0-9]+");
                    if (doesthepasswordcontainweirdcharsthatyoudontwantinapassword)
                    {
                        MessageBox.Show("Password may only contain letters and numers!");
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _prevCellValue;
                    }
                    else
                    {
                        _client.sendMessage(new Packet(_id + "m", "ManageUsers", "Server", "NewPass@" + id + "@" + newpass));
                        if (_client.ReadMessage()._message.Equals("Pass changed"))
                            MessageBox.Show("Password of client " + id + " successfully changed!");
                    }
                }
            }
        }

        // label update methodes
        private void DisplayLabelMessage(string message)
        {
            Error_Label.Text = message;
            _labelRemoveTimer.Start();
        }

        private void UpdateLabel(object sender, EventArgs e)
        {
            Error_Label.Text = "";
            _labelRemoveTimer.Stop();
        }

        // event handlers om het goed weer te geven
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            _prevCellValue = (string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            string value = (string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (value.Equals(_prevCellValue) || (string.IsNullOrEmpty(value)))
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _prevCellValue;
        }

        // onderstaande 2 methodes zijn om wachtwoorden in sterretjes weer te geven
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 1)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.PasswordChar = '*';
                }
            }

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (e.Value != null)
                {
                    e.Value = new string('*', e.Value.ToString().Length);
                }
            }
        }

        // stiekem wordt de window hidden ipv echt te sluiten
        private void ManageUsersTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}

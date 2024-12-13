using CSCWindowsLogsCollector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace CSCWindowsLogsCollector
{
    public partial class frmInsertLogs : Form
    {
        static BackgroundWorker backgroundWorker = new BackgroundWorker();
        static string pcName;
        static string pcIp;
        static connectionString=Settings.Default.connectionStringM;
        public frmInsertLogs()
        {
            InitializeComponent();
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
            backgroundWorker.WorkerReportsProgress = true;
        }
        static void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {        
            string logApp = "Application";
            string logSystem = "System";
            string computerName = Environment.MachineName;
            string ipAddress = GetLocalIPAddress();
            
            // EventLog eventLogApp = new EventLog(logApp);
            List<EventLogModel> eventLogApp = CollectorServices.GetTodayLogs(logApp, computerName, ipAddress);
            //EventLog eventLogSystem = new EventLog(lagSystem);
            List<EventLogModel> eventLogSystem = CollectorServices.GetTodayLogs(logSystem, computerName, ipAddress);
            int totalEntries = eventLogApp.Count+ eventLogSystem.Count;
            int processedEntries = 0;
            
            DateTime today = DateTime.Today;
             foreach (var log in eventLogApp)
            {
                //if (log.LogDate == today)
                //{
                if (!LogExists2(connectionString, log, logApp))
                {
                    //InsertLogIntoDatabase(log, logApp);
                    InsertLogIntoDatabase2(connectionString,log, logApp);
                }
                //}
                processedEntries++;
                int progressPercentage = (int)((double)processedEntries / totalEntries * 100);
                backgroundWorker.ReportProgress(progressPercentage);
            }

            foreach (var log in eventLogSystem)
            {
                //if (log.LogDate == today)
                //{
                if (!LogExists2(connectionString,log, logSystem))
                {
                    //InsertLogIntoDatabase(log, lagSystem);
                    InsertLogIntoDatabase2(connectionString, log, logSystem);
                }
                //}
                processedEntries++;
                int progressPercentage = (int)((double)processedEntries / totalEntries * 100);
                backgroundWorker.ReportProgress(progressPercentage);
            }             
        }       
       
        
        void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            lblProgress.Text = $"{e.ProgressPercentage}%";
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("Log insertion completed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //btnInsertLogs.Enabled = true;
            
        }
        private void button1_Click(object sender, EventArgs e)
        {                      
            try
            {
                //startButton.Enabled = false;
                backgroundWorker.RunWorkerAsync(); // Change to "System" or other log types if needed
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                //throw;
            }
            
        }

        static void InsertLogIntoDatabase(EventLogModel entry, string logType)
        {
            string query = @"
            INSERT INTO WindowsLogs (ServerName,ServerIP,LogType,LogDate, Source, EventID, EventType, Message)
            VALUES (@ServerName,@ServerIP,@LogType,@LogDate, @Source, @EventID, @EventType, @Message)";
            

            Cmd.Parameters["@ServerName"] = pcName;
            Cmd.Parameters["@ServerIP"] = pcIp;
            Cmd.Parameters["@LogType"] = logType;
            Cmd.Parameters["@LogDate"] = entry.LogDate;  
            Cmd.Parameters["@Source"] = entry.Source;
            Cmd.Parameters["@EventID"] = entry.EventID.ToString();
            Cmd.Parameters["@EventType"] = entry.EventType;
            Cmd.Parameters["@Message"] = entry.Message;

            Cmd.ExecuteNonQuery(query);
        }
        static bool LogExists(EventLogModel entry, string logType)
        {
            string query = @"
                SELECT COUNT(1)
                FROM WindowsLogs
                WHERE LogDate = @LogDate
                AND Source = @Source
                AND EventID = @EventID
                AND EventType = @EventType
                AND ServerName = @ServerName
                AND ServerIP = @ServerIP
                AND LogType =   @LogType";
            Cmd.Parameters["@ServerName"] = pcName;
            Cmd.Parameters["@ServerIP"] = pcIp;
            Cmd.Parameters["@LogType"] = logType;
            Cmd.Parameters["@LogDate"] = entry.LogDate;
            Cmd.Parameters["@Source"] = entry.Source;
            Cmd.Parameters["@EventID"] = entry.EventID.ToString();
            Cmd.Parameters["@EventType"] = entry.EventType;
            //Cmd.Parameters["@Message"] = entry.Message;

            int count = (int)Cmd.ExecuteScalar(query);
            return count > 0;
        }
        static void InsertLogIntoDatabase2(string connectionString, EventLogModel log, string logType)
        {
            string query = @"
                INSERT INTO WindowsLogs (ServerName,ServerIP,LogType,LogDate, Source, EventID, EventType, Message)
            VALUES (@ServerName,@ServerIP,@LogType,@LogDate, @Source, @EventID, @EventType, @Message)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ServerName", pcName);
                command.Parameters.AddWithValue("@ServerIP", pcIp);
                command.Parameters.AddWithValue("@LogType", logType);
                command.Parameters.AddWithValue("@LogDate", log.LogDate);
                command.Parameters.AddWithValue("@Source", log.Source);
                command.Parameters.AddWithValue("@EventID", log.EventID);
                command.Parameters.AddWithValue("@EventType", log.EventType);
                command.Parameters.AddWithValue("@Message", log.Message);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        static bool LogExists2(string connectionString, EventLogModel log, string logType)
        {
            string query = @"
                SELECT COUNT(1)
                FROM WindowsLogs
                WHERE LogDate = @LogDate
                AND Source = @Source
                AND EventID = @EventID
                AND EventType = @EventType
                AND ServerName = @ServerName
                AND ServerIP = @ServerIP
                AND LogType =   @LogType";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ServerName", pcName);
                command.Parameters.AddWithValue("@ServerIP", pcIp);
                command.Parameters.AddWithValue("@LogType", logType);
                command.Parameters.AddWithValue("@LogDate", log.LogDate);
                command.Parameters.AddWithValue("@Source", log.Source);
                command.Parameters.AddWithValue("@EventID", log.EventID);
                command.Parameters.AddWithValue("@EventType", log.EventType);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {           
            if (!backgroundWorker.IsBusy)
            {
                try
                {                    
                    backgroundWorker.RunWorkerAsync(); 

                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());                    
                }
            }
        }

        private void frmInsertLogs_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.isMinimize == true)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            if (Properties.Settings.Default.isStartup == true)
            {
                if (StartupShortcut.IsApplicationInStartup() == false)
                    StartupShortcut.AddApplicationToStartup();
            }
           
            if (Properties.Settings.Default.isMinimize == true)
            {
                cbMinimize.Checked = true;
            }
            else
            {
                cbMinimize.Checked = false;
            }

            if (Properties.Settings.Default.isStartup == true)
            {
                cbStartUp.Checked = true;
            }
            else
            {
                cbStartUp.Checked = false;
            }
            pcName = Environment.MachineName;
            pcIp = GetLocalIPAddress();
            try
            {
                Cmd.connection();
                lblIsOnline.Text = "Online";
                lblIsOnline.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblIsOnline.Text = "Offline";
                lblIsOnline.ForeColor = Color.Red;
                //throw;
            }
        }
        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Check if the close button was clicked or Alt+F4 was pressed
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // Cancel the close operation
                this.Hide(); // Hide the form
                notifyIcon1.Visible = true; // Show the NotifyIcon
            }
        }
        private void cbStartUp_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.isStartup = cbStartUp.Checked ? true : false;

            Properties.Settings.Default.Save();

            if (cbStartUp.Checked==false)
            {
                StartupShortcut.RemoveApplicationFromStartup();
            }
            else
            {
                StartupShortcut.AddApplicationToStartup();
            }
        }

        private void cbMinimize_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.isMinimize = cbMinimize.Checked ? true : false;

            Properties.Settings.Default.Save();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void frmInsertLogs_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void frmInsertLogs_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.isMinimize == true)
            {
                this.Hide();
                notifyIcon1.Visible = true; // Ensure the NotifyIcon is visible
                notifyIcon1.BalloonTipTitle = "CSCWindowsLogs Minimized";
                notifyIcon1.BalloonTipText = "The CSCWindowsLogs is running in the background.";
                notifyIcon1.ShowBalloonTip(2000); // Show notification for 3 seconds
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           //lblProgress.Text= Encryption.Decode("hT+Em5yR6MQosZCu1Q7LCY8/VxJh/iXzZvuZ18hYU5WQIgM8zz50Xgk+ahqVhiuIHTuRzaI80tRfyO1Fzw6ZkT4NRQh7JwAaVi7xgRDRSqLFiVGD5jZg756jgf90pmW/bMauuaDivzFm0CgXXPywWP4jtzBkXXhZK1/v747u96c=");
        }
    }
    
}

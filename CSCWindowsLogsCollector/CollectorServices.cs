using CSCWindowsLogsCollector.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSCWindowsLogsCollector
{
    public static class CollectorServices
    {
        public static List<EventLogModel> GetTodayLogs(string logType, string serverName, string serverIp)
        {
            EventLog eventLog = new EventLog(logType);
            DateTime today = DateTime.Today;
            var minDay = Settings.Default.minDay;
            minDay = minDay <= 0 ? minDay : minDay * -1;
            var todayLogs = eventLog.Entries.Cast<EventLogEntry>()
                .Where(entry => entry.TimeGenerated.Date >= today.AddDays(minDay))
                .Select(entry => new EventLogModel
                {
                    ServerName  = serverName,
                    ServerIP    = serverIp,
                    LogType     = logType,
                    LogDate     = entry.TimeGenerated,
                    Source      = entry.Source,
                    EventID     = (int)entry.InstanceId,
                    EventType   = entry.EntryType.ToString(),
                    Message     = entry.Message
                }).ToList();

            return todayLogs;
        }
    }

    public class EventLogModel
    {
        public string ServerName { get; set; }
        public string ServerIP { get; set; }
        public string LogType { get; set; }
        public DateTime LogDate { get; set; }
        public string Source { get; set; }
        public int EventID { get; set; }
        public string EventType { get; set; }
        public string Message { get; set; }
    }

    public static class LabelUpdater
    {
        public static void UpdateLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() => label.Text = text));
            }
            else
            {
                label.Text = text;
            }
        }
    }
}

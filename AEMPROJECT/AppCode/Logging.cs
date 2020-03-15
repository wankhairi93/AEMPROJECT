using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AEMPROJECT
{
    public class Logging
    {
        #region Variable Declaration
        private string m_strTextLogFile = string.Empty;
        private StreamWriter m_objTextLog;
        #endregion
        public Logging()
        {
            string strErrMsg = null;
            try
            {
                m_strTextLogFile = AppDomain.CurrentDomain.BaseDirectory + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".log";
                m_objTextLog = new StreamWriter(m_strTextLogFile, true);
            }
            catch (Exception ex)
            {
                strErrMsg = $"[LogEvents] Error in creating an instance of Logging class. \nEx-{ex.Message}";
                MessageBox.Show(strErrMsg, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public Logging(string strLogFile)
        {
            string strErrMsg = null;
            try
            {
                m_strTextLogFile = strLogFile;
                (new FileInfo(m_strTextLogFile)).Directory.Create();
                m_objTextLog = new StreamWriter(m_strTextLogFile, true);
            }
            catch (Exception ex)
            {
                strErrMsg = $"[LogEvents] Error in creating an instance of Logging class with log path. \nEx-{ex.Message}";
                MessageBox.Show(strErrMsg, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool Close()
        {
            string strErrMsg = null;
            try
            {
                return CloseTextLog();
            }
            catch (Exception ex)
            {
                strErrMsg = $"[LogEvents] Error in closing the Event Log File. \nEx-{ex.Message}";
                MessageBox.Show(strErrMsg, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
        private bool CloseTextLog()
        {
            string strErrMsg = null;
            try
            {
                if (m_objTextLog != null)
                {
                    m_objTextLog.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                strErrMsg = $"[LogEvents] Error in creating an instance of Logger class. \nEx-{ex.Message}";
                MessageBox.Show(strErrMsg, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return false;
        }
        public void WriteLine(string strLogMsg, System.Diagnostics.EventLogEntryType EvtType)
        {
            string strErrMsg = null;
            try
            {
                TxtLogWriteLine(strLogMsg, EvtType);
            }
            catch (Exception ex)
            {
                strErrMsg = $"[LogEvents] Error in creating an instance of Logger class. \nEx-{ex.Message}";
                MessageBox.Show(strErrMsg, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TxtLogWriteLine(string strLogMsg, System.Diagnostics.EventLogEntryType EvtType)
        {
            string strErrMsg = null;
            string strFriendlyName = null;
            try
            {
                if (m_objTextLog != null)
                {
                    switch (EvtType)
                    {
                        case System.Diagnostics.EventLogEntryType.Error:
                            {
                                strFriendlyName = "ERRO";
                                break;
                            }
                        case System.Diagnostics.EventLogEntryType.Information:
                            {
                                strFriendlyName = "INFO";
                                break;
                            }
                        case System.Diagnostics.EventLogEntryType.Warning:
                            {
                                strFriendlyName = "WARN";
                                break;
                            }
                    }
                    strLogMsg = $"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")} [{strFriendlyName}] - {strLogMsg}";
                    m_objTextLog.WriteLine(strLogMsg);
                    m_objTextLog.Flush();
                }
            }
            catch (Exception ex)
            {
                strErrMsg = $"[LogEvents] Error in creating an instance of Logger class. \nEx-{ex.Message}";
                MessageBox.Show(strErrMsg, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    public class Log
    {
        private static Logging m_EvtLog;
        public static bool InitLog()
        {
            string strErrMsg = null;
            string strFileName = string.Empty;
            try
            {
                strFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", DateTime.Today.ToString("yyyyMMdd"));
                strFileName = strFileName + "\\AEMEnersol.LOG";
                m_EvtLog = new Logging(strFileName);
                return true;
            }
            catch (Exception ex)
            {
                strErrMsg = "Error in intializing application log setting" + System.Environment.NewLine + ex.Message;
                MessageBox.Show(strErrMsg, "Main Application", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }
        public static void LogEvents(string strMsg, EventLogEntryType EvtType)
        {
            m_EvtLog.WriteLine(strMsg, EvtType);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace TCPConManageDll
{
    /// <summary>
    /// About this file
    /// </summary>
    public static class TCPConManageInfo
    {
        public const string Description = "By using this dll file, you can see which connections are using a specific port, and release the port by terminating related applications.";
        public const string Developer = "Morteza Mahmoudi";
        public const string EMail = "mortezadvlp@gmail.com";
    }

    /// <summary>
    /// By using this dll file, you can see which connections are using a specific port, and release the port by terminating related applications.
    /// </summary>
    public class TCPConnectionManager
    {
        /// <summary>
        /// Show which connections are using a specified port.
        /// </summary>
        /// <param name="port">Port number</param>
        /// <returns>List of Connections information</returns>
        public List<TCPConnectionItem> getTCpConnections(int port)
        {
            List<TCPConnectionItem> list = new List<TCPConnectionItem>();
            var ip = IPGlobalProperties.GetIPGlobalProperties();
            foreach (var tcp in ip.GetActiveTcpConnections())
            {
                if (tcp.LocalEndPoint.Port == port)
                {
                    list.Add(new TCPConnectionItem()
                    {
                        othersideIp = tcp.RemoteEndPoint.Address.ToString(),
                        othersidePort = tcp.RemoteEndPoint.Port,
                        state = tcp.State.ToString()
                    });
                }
            }
            foreach (var tcp in ip.GetActiveTcpListeners())
            {
                if (tcp.Port == port)
                {
                    list.Add(new TCPConnectionItem()
                    {
                        othersideIp = tcp.Address.ToString(),
                        othersidePort = tcp.Port,
                        state = "Listener"
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// Release a specified port by terminating applications using it.
        /// This function needs admin privilege for each termination.
        /// </summary>
        /// <param name="port">Port number</param>
        public void TerminateTCPConnections(int port)
        {
            Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C netstat -ano | findstr \":" + port.ToString() + "\"";
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            List<string> listOut = output.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string item in listOut)
            {
                if (item.ToLower().IndexOf("tcp") < 0)
                    continue;
                string[] xx = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string[] yy = xx[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (yy[1] != port.ToString())
                    continue;
                string pid = xx[xx.Length - 1];
                process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/C taskkill /PID " + pid + " /F";
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
            }
        }
    }
}

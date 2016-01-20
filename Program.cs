using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net;

namespace IPUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null)
            {
                LogError("No parameters passed!");
                return;
            }
            string user = null;
            string pass = null;
            string url = null;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-u":
                    case "-user":
                        i++;
                        if (args.Length <= i)
                        {
                            LogError("incorrect argument syntax!");
                            return;
                        }
                        user = args[i];
                        break;
                    case "-p":
                    case "-password":
                        i++;
                        if (args.Length <= i)
                        {
                            LogError("incorrect argument syntax!");
                            return;
                        }
                        pass = args[i];
                        break;
                    case "-s":
                    case "-url":
                        i++;
                        if (args.Length <= i)
                        {
                            LogError("incorrect argument syntax!");
                            return;
                        }
                        url = args[i];
                        break;
                }
            }
            string newIp;
            try
            {
                var wc = new WebClient();
                newIp = wc.DownloadString("http://icanhazip.com/").Trim();
            }
            catch
            {
                LogError("failed to retrieve IP");
                return;
            }
            if (!File.Exists("IP.txt"))
            {
                using (StreamWriter writer = new StreamWriter("IP.txt"))
                {
                    writer.WriteLine(newIp);
                    writer.WriteLine("Retrieved " + DateTime.Now.ToString("s"));
                }
            }
            else
            {
                string oldIp;
                using (StreamReader reader = new StreamReader("IP.txt"))
                {
                    oldIp = reader.ReadLine();
                }
                if (String.IsNullOrWhiteSpace(oldIp))
                {
                    using (StreamWriter writer = new StreamWriter("IP.txt"))
                    {
                        writer.WriteLine(newIp);
                        writer.WriteLine("Retrieved " + DateTime.Now.ToString("s"));
                    }
                    return;
                }
                if (String.Equals(newIp, oldIp)) { return; }
                using (StreamWriter writer = new StreamWriter("IP.txt"))
                {
                    writer.WriteLine(newIp);
                    writer.WriteLine("Retrieved " + DateTime.Now.ToString("s"));
                }
            }
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(user.Normalize(), pass.Normalize());
                request.Method = WebRequestMethods.Ftp.UploadFile;
                StreamReader sourceStream = new StreamReader("IP.txt");
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
            }
            catch (Exception e)
            {
                LogError("failed to upload file to server.");
                LogError(e.ToString());
                return;
            }
        }
        static void LogError(string e)
        {
            if (!File.Exists("IP.log"))
            {
                File.Create("IP.log");
            }
            using (StreamWriter writer = new StreamWriter("IP.log", true))
            {
                writer.WriteLine(DateTime.Now.ToString("s") + " ERROR: " + e);
            }
        }
    }
}


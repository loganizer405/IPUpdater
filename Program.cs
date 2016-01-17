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
                        if (args.Length < i)
                        {
                            LogError("incorrect argument syntax!");
                            return;
                        }
                        user = args[i];
                        break;
                    case "-p":
                    case "-password":
                        i++;
                        if (args.Length < i)
                        {
                            LogError("incorrect argument syntax!"); 
                            return;
                        }
                        pass = args[i];
                        break;
                    case "-s":
                    case "-url":
                        i++;
                        if (args.Length < i)
                        {
                            LogError("incorrect argument syntax!"); 
                            return;
                        }
                        url = args[i];
                        break;
                }
            }
            if (args == null)
            {
                LogError("No parameters passed!");
                return;
            }
            try
            {
                var wc = new WebClient();
                string newIp = wc.DownloadString("http://icanhazip.com/");
                if (!File.Exists("IP.txt"))
                {
                    File.Create("IP.txt");
                    StreamWriter writer = new StreamWriter("IP.txt");
                    {
                        writer.Write(newIp);
                    }
                }
                else
                {
                    string oldIp = File.ReadAllText("IP.txt");
                    if (newIp == oldIp)//IP did not change, exiting program.
                        return;
                    //IP changed:        
                    File.WriteAllText("IP.txt", newIp);
                }
            }
            catch
            {
                LogError("failed to retrieve IP");
                return;
            }
            try
            {
                //uploading file
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, pass);
                StreamReader sourceStream = new StreamReader("IP.txt");
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch
            {
                LogError("failed to upload file to server.");
                return;
            }
        }
        static void LogError(string e)
        {
            if (!File.Exists("IP.log"))
            {
                File.Create("IP.log");
            }
            StreamWriter writer = new StreamWriter("IP.log");
            {
                writer.WriteLine(DateTime.Now.ToString("u") + "ERROR: " + e);
            }
        }
    }
}


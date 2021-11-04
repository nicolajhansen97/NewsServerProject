using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MVVMStart.Model;
using MVVMStart.ViewModel;

namespace MVVMStart
{
    class ConnectionModel : Bindable
    {

        static TcpClient socket = new TcpClient();

        static String response;
        static byte[] downBuffer = new byte[2048];
        static byte[] byteSendInfo = new byte[2048];

        static int bytesSize;
        static String NewChunk;
        static NetworkStream ns = null;

        public static void MakeConnection(string hostname, int port, string username, string password)
        {
            try {
                //(0) check the ip via DNS first
                IPAddress ip = Dns.GetHostEntry(hostname).AddressList[0];
                IPEndPoint endPoint = new IPEndPoint(ip, port);

                //(1) this blocks until connection is reached or fails
                socket.Connect(endPoint);

                //(2) get the streams
                ns = socket.GetStream();

                Console.WriteLine("Connected to the remote server, the stream is ready... the ip of the remote host is:" + ip.ToString());

                int test = ns.Read(downBuffer, 0, 2048);
                response = System.Text.Encoding.ASCII.GetString(downBuffer, 0, test);
                if (response.Substring(0, 3) != "200")
                {
                    Console.WriteLine("The server returned an unexpected response.", "Connection failed");
                }
                Console.WriteLine(response + "\n");

                response = System.Text.Encoding.ASCII.GetString(downBuffer, 0, test);
                byteSendInfo = StringToByteArr("AUTHINFO USER " + username + "\r\n");

                ns.Write(byteSendInfo, 0, byteSendInfo.Length);

                test = ns.Read(downBuffer, 0, 2048);
                response = System.Text.Encoding.ASCII.GetString(downBuffer, 0, test); ;

                Console.WriteLine(response + "\n");

                byteSendInfo = StringToByteArr("AUTHINFO PASS " + password + "\r\n");

                ns.Write(byteSendInfo, 0, byteSendInfo.Length);

                test = ns.Read(downBuffer, 0, 2048);
                response = System.Text.Encoding.ASCII.GetString(downBuffer, 0, test); ;

                Console.WriteLine(response + "\n");

                ConnectViewModel.connectedBool = true;

                getList();
            }
            catch (Exception Ex)
            {
                // MessageBox.Show(Ex.ToString());
                ConnectViewModel.connectedBool = false;
                MessageBox.Show("Some of your information is wrong!");

            }
        }

        public static void getList()
        {

            byteSendInfo = StringToByteArr("list\r\n");

            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            response = "";

            while ((bytesSize = ns.Read(downBuffer, 0, downBuffer.Length)) > 0)
            {

                // Get the chunk of string

                NewChunk = Encoding.ASCII.GetString(downBuffer, 0, bytesSize);

                response += NewChunk;

                // If the string ends in a "\r\n.\r\n" then the list is over

                if (NewChunk.Substring(NewChunk.Length - 5, 5) == "\r\n.\r\n")

                {

                    // Remove the "\r\n.\r\n" from the end of the string

                    response = response.Substring(0, response.Length - 3);

                    break;

                }

            }
            string[] ListLines = response.Split('\n');

            foreach (String ListLine in ListLines)
            {
                NewsViewModel.NewsServerList.Add(MakeList(ListLine));

                //Console.WriteLine(ListLine + "\n");
            }

        }

        private static NewsServerModel MakeList(string list)
        {
            NewsServerModel nsm = new NewsServerModel();

            nsm.NewsServerName = list;
            //MessageBox.Show(nsm.NewsServer); Gets the newsservers?
            return nsm;
        }

        public static void getArticles(string useGroupName)
        {
            byteSendInfo = StringToByteArr("GROUP " + useGroupName + "\r\n");

            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            response = "";

            bytesSize = ns.Read(downBuffer, 0, 2048);

            response = System.Text.Encoding.ASCII.GetString(downBuffer, 0, bytesSize);

            // Split the information about the newsgroup by blank spaces

            string[] Group = System.Text.Encoding.ASCII.GetString(downBuffer, 0, bytesSize).Split(' ');

            // The ID of the first article in this newsgroup

            int firstID = Convert.ToInt32(Group[2]);

            // The ID of the last article in this newsgroup

            int lastID = Convert.ToInt32(Group[3]);

            for (int i = firstID; i < lastID; i++)
            {
                byteSendInfo = StringToByteArr("XOVER " + i + "\r\n");

                ns.Write(byteSendInfo, 0, byteSendInfo.Length);

                response = "";

                bytesSize = ns.Read(downBuffer, 0, 2048);

                response = System.Text.Encoding.ASCII.GetString(downBuffer, 0, bytesSize);

                string[] ListLines = response.Split('\r', '\n');

                // string heading = response.Replace("224 data follows", string.Empty);

                // MessageBox.Show(heading);

                foreach (string ListLine in ListLines)
                {
                    if (!ListLine.Contains("224 data follows") && !ListLine.Equals(".") && !ListLine.Equals(""))

                   
                        NewsViewModel.ArticleList.Add(GenerateHeadings(ListLine));
                }
            }

        }

        public static void getArticleText(int articleNumber)
        {

   
            byteSendInfo = StringToByteArr("BODY " + articleNumber + "\r\n");
            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            response = "";

            while ((bytesSize = ns.Read(downBuffer, 0, downBuffer.Length)) > 0)
            {
                // Get the chunk of string
                NewChunk = Encoding.ASCII.GetString(downBuffer, 0, bytesSize);
                response += NewChunk;
                // If the string ends in a "\r\n.\r\n" then the list is over
                if (NewChunk.Substring(NewChunk.Length - 5, 5) == "\r\n.\r\n")
                {
                    // Remove the "\r\n.\r\n" from the end of the string
                    response = response.Substring(0, response.Length - 3);
                    break;
                }
            }
           
            MessageBox.Show(response);
        }
    




        private static ArticleModel GenerateHeadings(string headingName)
        {
            ArticleModel am = new ArticleModel();

            am.ArticleHeadline = headingName;
           
            return am;
        }

        public static void postArticle(string userName, string group, string subject, string message)
        {
            byteSendInfo = StringToByteArr("POST\r\n");

            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            byteSendInfo = StringToByteArr("FROM " + userName + "\r\n");

            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            byteSendInfo = StringToByteArr("NEWSGROUPS " + group + "\r\n");

            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            byteSendInfo = StringToByteArr("SUBJECT " + subject + "\r\n");

            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            byteSendInfo = StringToByteArr("MESSAGE " + message + "\r\n");

            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            byteSendInfo = StringToByteArr(".\r\n");

            ns.Write(byteSendInfo, 0, byteSendInfo.Length);

            bytesSize = ns.Read(downBuffer, 0, 2048);


            response = System.Text.Encoding.ASCII.GetString(downBuffer, 0, bytesSize);

            MessageBox.Show(response);
        }
        public static byte[] StringToByteArr(string str)

        {

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            return encoding.GetBytes(str);

        }

    }
}
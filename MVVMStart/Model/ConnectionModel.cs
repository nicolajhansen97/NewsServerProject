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

        

        List<string> AllServers = new List<string>();

        TcpClient socket = new TcpClient();

        static String response;
        static byte[] downBuffer = new byte[2048];
        static byte[] byteSendInfo = new byte[2048];

        int bytesSize;
        String NewChunk;
        NetworkStream ns = null;
        StreamReader reader = null;
        StreamWriter writer = null;

        public void MakeConnection(string hostname, int port, string username, string password)
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

        public void getList()
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

                Console.WriteLine(ListLines.Length);

             
                foreach (String ListLine in ListLines)
                {

               // NewsServerList.Add(new NewsServerModel() { NewsServerName = ListLine });
                NewsViewModel.NewsServerList.Add(MakeList(ListLine));
              
                
                    //Console.WriteLine(ListLine + "\n");
                   // MessageBox.Show(ListLine);
                }
        }

        
        private NewsServerModel MakeList(string list)
        {
            NewsServerModel nsm = new NewsServerModel();

            nsm.NewsServerName = list;
            //MessageBox.Show(nsm.NewsServer); Gets the newsservers?
            return nsm;
        }
        public static byte[] StringToByteArr(string str)

        {

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            return encoding.GetBytes(str);

        }

    }
}
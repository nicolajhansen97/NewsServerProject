using MVVMStart.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MVVMStart.ViewModel
{

    
    class ConnectViewModel : Model.Bindable

    {
        public NewsViewModel TDFVM { get; set; }

        string file = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + "/NewsProgram/LoginSaved.txt";
        string dir = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + "/NewsProgram";
        public DelegateCommand quitProgram { get; set; }
        public DelegateCommand programLogin { get; set; }
        public DelegateCommand saveUserInfo { get; set; }

        public static bool connectedBool;

        private string saveButton = "Save";

        public string SaveButton
        {
            get { return saveButton; }
            set { saveButton = value; propertyIsChanged(); }
        }

        private string hostName;

        public string HostName
        {
            get { return hostName; }
            set { hostName = value; propertyIsChanged(); }
        }

        private int port;

        public int Port
        {
            get { return port; }
            set { port = value; propertyIsChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; propertyIsChanged(); }
        }

        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public ConnectViewModel()
        {

            quitProgram = new DelegateCommand(o =>
            {
                QuitProgram();
            });

            programLogin = new DelegateCommand(o =>
            {
             
                ProgramLogin();

                  if (connectedBool)
                {
                    ChangeToNewsView();
                    
                 
                }

            });

            saveUserInfo = new DelegateCommand(o =>
            {
                SaveData();
            });

            CheckSavedData();

        }
        private void QuitProgram()
        {
            Application.Current.Shutdown();
        }

        private void ProgramLogin()
        {
          //  ConnectionModel connection = new ConnectionModel();
            ConnectionModel.MakeConnection(HostName, Port, Username, Password);
        }

        private void CheckSavedData()
        {
            if (File.Exists(file))
            {
                try
                {
                    string[] savedInfo = File.ReadAllLines(file);

                    HostName = savedInfo[0];
                    Port = Int32.Parse(savedInfo[1]);
                    Username = savedInfo[2];
                    Password = savedInfo[3];

                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString());
                }
            }
        }
        
        private void SaveData()
        {

            try
                {

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(file))
                     { 
                    File.Delete(file);
                     }
                    {
                        using (StreamWriter sw = File.CreateText(file))
                        {
                            sw.WriteLine(string.Join(Environment.NewLine, HostName));
                            sw.WriteLine(string.Join(Environment.NewLine, Port));
                            sw.WriteLine(string.Join(Environment.NewLine, Username));
                            sw.WriteLine(string.Join(Environment.NewLine, Password));
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString());
                }
            }
        private void ChangeToNewsView()
        {
            MainViewModel current = MainViewModel.current;
            TDFVM = new NewsViewModel();
            current.CurrentView = TDFVM;
        }
        }
    }



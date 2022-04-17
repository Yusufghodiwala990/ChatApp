// class which connects the xml and has the logic and structure for messages and buttons on the windows form app
using chatApp.MVVM.Core;
using chatApp.MVVM.Model.User;
using chatApp.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace chatApp.MVVM.ViewModel
{
    class MainViewModel
    {

        // relay commands which are bound to the connect and send buttons on the windows form
        public RelayCommand ConnectToServerCommand { get; set; }

        public RelayCommand SendMessageCommand { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }  
        // username and message to be sent
        public string username { get; set; }
        public string message { get; set; }
        private server _server;
        public MainViewModel()
        {
            // create a list of users and add them whenever a new one connects
            Users = new ObservableCollection<UserModel>();

            // collection for all the messages that have been sent
            Messages = new ObservableCollection<string>();

            // create the server instance which provides client to server functionality
            _server = new server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += RemoveUser;

            // call the relay command based on the connect/send button
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(username),o =>!string.IsNullOrEmpty(username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(message), o => !string.IsNullOrEmpty(message));
        }

        // method to remove user when the disconnection
        private void RemoveUser()
        {
            // read their uid before removing
            var uid = _server.PacketReader.ReadMessage();

            // remove them from the collection using LINQ
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        // method to read a message and adding them to the list of messages in a collection
        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        // print out the user's uid and username when a user connects to the server
        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };

            if(!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
             
            }
        }
    }
} 

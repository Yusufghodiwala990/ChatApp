// class which holds the logic for the server from the server perspective
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using chatServer.Net.IO;
using System.Linq;

namespace chatServer

{
    class Program
    {
        // create a listener instance
        static TcpListener _listener;
        static List<Client> _users; // list to hold all the users who are connected
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 22000); // listen on port 22000
            _listener.Start();

            // infinite loop to keep accepting clients
            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);
                BroadcastConnection(); // broadcast their connection for all the users to see the messages being sent by the new connected user
            }
        }
        // method to broadcast their connection for all the users to see the messages being sent by the new connected user
        static void BroadcastConnection()
        {
            // for each user
            foreach(var user in _users)
            {
                // get the client of each user
                foreach(var usr in _users)
                {
                    // build packets and indicate that they have connected to the session
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteString(usr.username);
                    broadcastPacket.WriteString(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.getPacketBytes());
                }
            }
        }

        // method to broadcast messages
        public static void BroadCastMessage(string message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteString(message);
                user.ClientSocket.Client.Send(msgPacket.getPacketBytes());

            }
        }

        // method to broadcast(print) that a user has exited a session
        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteString(uid);
                user.ClientSocket.Client.Send(broadcastPacket.getPacketBytes());
                

            }

            BroadCastMessage($"[{disconnectedUser.username}] Disconnected");
        }

    }
}

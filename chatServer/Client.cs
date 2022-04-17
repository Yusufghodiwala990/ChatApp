// class which holds the logic of the client by the server
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using chatServer.Net.IO;

namespace chatServer
{
    class Client
    {
        // variables to hold user details
        public string username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }  // tcp_client instance


        PacketReader _packetReader;

        // build the client
        public Client(TcpClient client)
        {
            // read the messages sent by client and print them out
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());
            var opcode =  _packetReader.ReadByte();
            username = _packetReader.ReadMessage();
           
            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username:{username}");
            Task.Run(() => Process());
        }

        void Process()
        {
            while(true)
            {
                // keep running the server for clients to connect and print out messages sent by clients
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch(opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message Received {msg}");
                            Program.BroadCastMessage(msg);
                            break;
                    }

                }
                // exceptions when the client closes their window, print their UID and a disconnect message
                catch (Exception)
                {
                    Console.WriteLine($"[{UID}: DISCONNECTED");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                    
                }
            }
        }
    }
}

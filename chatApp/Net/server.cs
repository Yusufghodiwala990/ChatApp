// server logic for clients
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using chatApp.Net.IO;
using System.IO;

namespace chatApp.Net
{
    class server
    {

        // create instance for the tcp_client
        TcpClient _client;
        public PacketReader PacketReader; // packet reader instance

        // actions that are triggered upon meeting a condition
        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnectEvent;

        class PacketBuilder
        {
            MemoryStream _ms; // create memory stream
            public PacketBuilder()
            {
                _ms = new MemoryStream();
            }


            // method to write out the opcode
            public void WriteOpCode(byte opcode)
            {
                _ms.WriteByte(opcode);
            }

            // method to write out the message
            public void WriteString(string msg)
            {
                var msgLength = msg.Length;
                _ms.Write(BitConverter.GetBytes(msgLength));
                _ms.Write(Encoding.ASCII.GetBytes(msg));
            }

            // method to get the message bytes in an array
            public byte[] getPacketBytes()
            {
                return _ms.ToArray();
            }
        }
        public server()
        {
            _client = new TcpClient();

        }

        // method that actually connections the client to the server
        public void ConnectToServer(string username)
        {
            if(!_client.Connected)
            {
                _client.Connect("127.0.0.1", 22000);    // port 22000 is used by the server to listen, thus we connect the tcp_client here
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    // build the connected message packet and send it to the server
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteString(username);
                    _client.Client.Send(connectPacket.getPacketBytes());
                }
                ReadPackets();
            }
        }

        // method that reads the opcode of packets and invokes appropriate methods
        private void ReadPackets()
        {
            // tasks
            Task.Run(() =>
            {
                while(true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("Yes!!");
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                    }

                }
            });

        }
        // method to send a message to the server by building a packet
        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteString(message);
            _client.Client.Send(messagePacket.getPacketBytes());
        }
    }
}

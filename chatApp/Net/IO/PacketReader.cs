// class to read packets for the client side
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace chatApp.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream _ns;   // create a network stream
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }
        public string ReadMessage()
        {

            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            _ns.Read(msgBuffer, 0, length); // call read and populate the buffer with the bytes and the retrieve the string by using the Encoding class
            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }
    }
}

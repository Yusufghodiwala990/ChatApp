// class to build packets in bytes with op codes for the server to interprete (client side)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace chatApp.Net.IO
{
    class PacketBuilder
    {
        // create a memory stream for the packet
        MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode); // write out the op code to the memory stream
        }
        
        // get the message being sent
        public void WriteString(string msg)
        {
            var msgLength = msg.Length;
            _ms.Write(BitConverter.GetBytes(msgLength)); // get the message in bytes and encoding it
            _ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        // get the bytes in an array
        public byte[] getPacketBytes()
        {
            return _ms.ToArray(); 
        }
    }
}

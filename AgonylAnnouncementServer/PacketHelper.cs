using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgonylAnnouncementServer
{
    public static class PacketHelper
    {
        /// <returns></returns><summary>
        /// Takes the hex string and converts them to array of bytes
        /// each hex should be seperated by <space>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] MakeBytesArrayfromHexString(string data)
        {
            var hex = data;
            var trimHexString = hex.Trim();
            var stringHexArray = trimHexString.Split(new char[] { ' ' });
            var intList = new List<int>();

            // Convert to int array
            for (var i = 0; i < stringHexArray.Length; i++)
            {
                var k = Convert.ToInt32(stringHexArray[i], 16);
                intList.Add(k);
            }

            var intArray = intList.ToArray();

            return intArray.Select(i => (byte)i).ToArray();
        }

        ///
        /// <returns></returns><summary>
        /// Takes the integer string and converts them to array of bytes
        /// each int should be seperated by <space>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] MakeBytesArrayfromIntString(string data, char seprator = ' ')
        {
            var hex = data;
            var trimHexString = hex.Trim();
            var stringHexArray = trimHexString.Split(seprator);
            var intList = new List<int>();
            var intArray = stringHexArray.Select(int.Parse).ToArray();
            var bytes = intArray.Select(i => (byte)i).ToArray();
            return bytes;
        }

        /// <summary>
        /// Combining 2 byte array
        /// </summary>
        /// <param name="a">byte array 1</param>
        /// <param name="b">byte array 2</param>
        /// <returns>returns combined byte array (byte array 1 + byte array 2)</returns>
        public static byte[] CombineByteArray(byte[] a, byte[] b)
        {
            var c = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, c, 0, a.Length);
            Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }

        /// <summary>
        /// getting byte[] from string
        /// </summary>
        /// <param name="str">string data</param>
        /// <returns>byte[] obtained from string data</returns>
        public static byte[] GetBytesFrom(string str)
        {
            var bytes = Encoding.Default.GetBytes(str);
            return bytes;
        }

        public static byte[] GetZeroHexPacket(int num)
        {
            var strZero = "0";
            for (var i = 0; i < num - 1; i++)
            {
                strZero = strZero + "," + "0";
            }

            return MakeBytesArrayfromIntString(strZero, ',');
        }

        public static byte[] BuildAnnouncementPacket(string name, string message, string type = "245")
        {
            // 1,161,116,0,choice,173,32,0,0,0 //Yellow Msg
            // 1,161,116,0,245,173,32,0,0,0 //Announce choice 245 @WSHOUT Green, 0 is normal shout, 241 is player shout
            var shoutPacket1 = MakeBytesArrayfromIntString("1,161,116,0," + type + ",173,32,0,0,0", ',');
            var gmnametobytes = GetBytesFrom(name);
            var addzero = 42 - gmnametobytes.Length;
            gmnametobytes = CombineByteArray(gmnametobytes, GetZeroHexPacket(addzero));
            shoutPacket1 = CombineByteArray(shoutPacket1, gmnametobytes);
            var msgToBytes = GetBytesFrom(message);
            var addzero2 = 64 - message.Length;
            msgToBytes = CombineByteArray(msgToBytes, GetZeroHexPacket(addzero2));
            return CombineByteArray(shoutPacket1, msgToBytes);
        }
    }
}

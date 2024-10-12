using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TrezorLib
{
    class Utils
    {
        static byte[] ProtoSerialize<T>(T record) where T : class
        {
            if (null == record) return null;

            try
            {
                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, record);
                    return stream.ToArray();
                }
            }
            catch
            {
                // Log error
                throw;
            }
        }

        public static T ProtoDeserialize<T>(byte[] data) where T : class
        {
            if (null == data) return null;

            try
            {
                using (var stream = new MemoryStream(data))
                {
                    return ProtoBuf.Serializer.Deserialize<T>(stream);
                }
            }
            catch
            {
                // Log error
                throw;
            }
        }

        static byte[] FormatAsHIDPackets(byte[] msg, MessageType messageType)
        {
            var buffer = new byte[msg.Length + 8];

            buffer[0] = 35;
            buffer[1] = 35;

            buffer[2] = (byte)(((int)messageType >> 8) & 0xFF);
            buffer[3] = (byte)(((int)messageType >> 0) & 0xFF);

            Int32 size = msg.Length;
            buffer[4] = (byte)((size >> 24) & 0xFF);
            buffer[5] = (byte)((size >> 16) & 0xFF);
            buffer[6] = (byte)((size >> 8) & 0xFF);
            buffer[7] = (byte)((size >> 0) & 0xFF);

            msg.CopyTo(buffer, 8);

            return buffer;
        }

        public static void Write<T>(T message, MessageType messageType) where T : class
        {
            System.Diagnostics.Debug.WriteLine("TREZOR Writing: {0}", messageType);
            Write(ProtoSerialize<T>(message), messageType);
        }

        static void Write(byte[] msg, MessageType messageType)
        {
            var toWrite = FormatAsHIDPackets(msg, messageType);

            int packets = (toWrite.Length / 63) + 1;

            for (int i = 0; i < packets; i++)
            {
                var buffer = new byte[65];

                buffer[1] = 63;

                toWrite.Skip(i * 63).Take(63).ToArray().CopyTo(buffer, 2);

                Device.EndPoint.Write(buffer);
            }
        }

        static byte[] ReadChunk()
        {
            var r = Device.EndPoint.Read();
            return r.Data;
        }

        public static byte[] Read(out MessageType messageType)
        {
            //Read a chunk
            var readBuffer = ReadChunk();

            //Check to see that this is a valid first chunk 
            if (readBuffer[1] != (byte)'?' || readBuffer[2] != (byte)'#' || readBuffer[3] != (byte)'#')
            {
                throw new Exception("Bad read");
            }

            messageType = (MessageType)readBuffer[5];

            var totalDataLength = ((readBuffer[6] & 0xFF) << 24)
                                      + ((readBuffer[7] & 0xFF) << 16)
                                      + ((readBuffer[8] & 0xFF) << 8)
                                      + (readBuffer[9] & 0xFF);

            var remainingDataLength = totalDataLength;
            var length = Math.Min(readBuffer.Length - 10, remainingDataLength);

            List<byte> allData = new List<byte>();
            //This is the first chunk so read from 10 to end
            allData.AddRange(readBuffer.Skip(10).Take(length));

            remainingDataLength -= length;

            int invalidChunksCounter = 0;

            while (remainingDataLength > 0)
            {
                //Read a chunk
                readBuffer = ReadChunk();

                //check that there was some data returned
                if (readBuffer.Length <= 0)
                {
                    continue;
                }

                //Check what's smaller, the buffer or the remaining data length
                length = Math.Min(readBuffer.Length - 2, remainingDataLength);

                if (readBuffer[1] != (byte)'?')
                {
                    if (invalidChunksCounter++ > 5)
                    {
                        throw new Exception("messageRead: too many invalid chunks (2)");
                    }
                }

                allData.AddRange(readBuffer.Skip(2));

                //Decrement the length of the data to be read
                remainingDataLength -= (length);
            }

            return allData.Take(totalDataLength).ToArray();
        }        
    }
}

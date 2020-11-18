﻿using GenICam;
using GigeVision.Core.Models;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GigeVision.Core
{
    public class GenPort : IGenPort
    {
        public GenPort(int port)
        {
            IP = "192.168.10.244";
            Port = port;
            Gvcp = new Gvcp(this);
        }

        public Gvcp Gvcp { get; }
        public string IP { get; set; }
        public int Port { get; }

        public async Task<IReplyPacket> Read(long address, long length)
        {
            var addressBytes = GetAddressBytes(address, length);
            Array.Reverse(addressBytes);

            GvcpReply reply = new GvcpReply();

            if (length < 4)
                return reply;

            if (length >= 8)
                return await Gvcp.ReadMemoryAsync(IP, addressBytes, (ushort)length);
            else
                return await Gvcp.ReadRegisterAsync(addressBytes);
        }

        public async Task<IReplyPacket> Write(byte[] pBuffer, long address, long length)
        {
            await Gvcp.TakeControl(false);

            var addressBytes = GetAddressBytes(address, length);
            Array.Reverse(addressBytes);

            return await Gvcp.WriteRegisterAsync(addressBytes, BitConverter.ToUInt16(pBuffer));
        }

        private byte[] GetAddressBytes(Int64 address, Int64 length)
        {
            switch (length)
            {
                case 2:
                    return BitConverter.GetBytes((Int16)address);

                case 4:
                    return BitConverter.GetBytes((Int32)address);

                default:
                    break;
            }
            return BitConverter.GetBytes((Int32)address);
        }
    }
}
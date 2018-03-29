using dCom.Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace dCom.Modbus.ModbusFunctions
{
	public class ReadInputRegistersFunction : ModbusFunction
	{
		public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
		{
			CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
		}

		public override byte[] PackRequest()
		{
            byte[] buffer = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CommandParameters.TransactionId)), 2, buffer, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CommandParameters.ProtocolId)), 2, buffer, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CommandParameters.Length)), 2, buffer, 4, 2);
            buffer[6] = CommandParameters.UnitId;
            buffer[7] = CommandParameters.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(((ModbusReadCommandParameters)CommandParameters).StartAddress)), 2, buffer, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(((ModbusReadCommandParameters)CommandParameters).Quantity)), 2, buffer, 10, 2);
            return buffer;
        }

		public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
		{
            byte[] buffer = new byte[response[8]];
            int n = response[8] / 2;
            Dictionary<Tuple<PointType, ushort>, ushort> dictionary = new Dictionary<Tuple<PointType, ushort>, ushort>();
            for (int i = 0; i < n; ++i)
            {
                byte[] temp = new byte[2];
                Buffer.BlockCopy(response, 9 + i * 2, temp, 0, 2);
                ushort value = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(temp, 0));
                dictionary.Add(new Tuple<PointType, ushort>(PointType.IN_REG, ((ModbusReadCommandParameters)CommandParameters).StartAddress++), value);
            }
            return dictionary;
        }
	}
}

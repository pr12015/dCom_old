using dCom.Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace dCom.Modbus.ModbusFunctions
{
	public class WriteSingleCoilFunction : ModbusFunction
	{
		public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
		{
			CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
		}

		public override byte[] PackRequest()
		{
            byte[] buffer = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CommandParameters.TransactionId)), 2, buffer, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CommandParameters.ProtocolId)), 2, buffer, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CommandParameters.Length)), 2, buffer, 4, 2);
            buffer[6] = CommandParameters.UnitId;
            buffer[7] = CommandParameters.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(((ModbusWriteCommandParameters)CommandParameters).OutputAddress)), 2, buffer, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(((ModbusWriteCommandParameters)CommandParameters).Value)), 2, buffer, 10, 2);
            return buffer;
        }

		public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
		{
            byte[] buffer = new byte[2];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(response[10])), 0, buffer, 0, 2);
            Tuple<PointType, ushort> t = new Tuple<PointType, ushort>(PointType.DI_REG,((ModbusWriteCommandParameters)CommandParameters).OutputAddress);
            Dictionary<Tuple<PointType, ushort>, ushort> return_dictionary = new Dictionary<Tuple<PointType, ushort>, ushort>();
            return_dictionary.Add(t, ((ModbusWriteCommandParameters)CommandParameters).Value);
            return return_dictionary;
        }
	}
}

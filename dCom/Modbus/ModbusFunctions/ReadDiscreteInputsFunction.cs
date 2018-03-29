using dCom.Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace dCom.Modbus.ModbusFunctions
{
	public class ReadDiscreteInputsFunction : ModbusFunction
	{
		public ReadDiscreteInputsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
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
            int number_of_reg = response[8];
            int n = number_of_reg / 8 + (number_of_reg % 8 == 0 ? 0 : 1);
            byte[] buffer = new byte[n];
            Dictionary<Tuple<PointType, ushort>, ushort> dic = new Dictionary<Tuple<PointType, ushort>, ushort>();
            int j = 0;
            for(int i = 0; i < number_of_reg; ++i)
            {
                if (i % 8 == 0)
                    ++j;
                int value = buffer[j] >> i;
                value &= 1;
                dic.Add(new Tuple<PointType, ushort>(PointType.IN_REG, ((ModbusReadCommandParameters)CommandParameters).StartAddress++), (ushort)value);
            }

            return dic;
		}
	}
}

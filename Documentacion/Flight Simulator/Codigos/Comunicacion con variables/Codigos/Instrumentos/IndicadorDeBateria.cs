using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class IndicadorBateria
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            // Variables de carga de batería
            simconnect.AddToDataDefinition(DEFINITIONS.BatteryData, "ELECTRICAL MAIN BUS VOLTAGE", "volts", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.BatteryData, "ELECTRICAL MAIN BUS AMPS", "amperes", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.BatteryData, "ELECTRICAL BATTERY LOAD", "amperes", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<BatteryData>(DEFINITIONS.BatteryData);
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.BatteryData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en Indicador de Bateria: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en Indicador de Bateria: " + ex.Message);
        }
    }

    public void ReceiveMessage()
    {
        try
        {
            simconnect?.ReceiveMessage();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al recibir mensaje en Indicador de Bateria: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var batteryData = (BatteryData)data.dwData[0];
            Console.WriteLine($"Voltaje del bus principal: {batteryData.MainBusVoltage} volts");
            Console.WriteLine($"Amperaje del bus principal: {batteryData.MainBusAmps} amperes");
            Console.WriteLine($"Carga de la batería: {batteryData.BatteryLoad} amperes");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos del Indicador de Bateria: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { BatteryData }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct BatteryData
    {
        public double MainBusVoltage;
        public double MainBusAmps;
        public double BatteryLoad;
    }
}

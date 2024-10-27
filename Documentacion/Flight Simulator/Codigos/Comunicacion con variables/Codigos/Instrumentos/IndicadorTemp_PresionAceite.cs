using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class IndicadorTemp_PresionAceite
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            // Variables de presión y temperatura del aceite
            simconnect.AddToDataDefinition(DEFINITIONS.OilData, "GENERAL ENG OIL PRESSURE:1", "psi", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.OilData, "GENERAL ENG OIL TEMPERATURE:1", "celsius", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<OilData>(DEFINITIONS.OilData);
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.OilData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorTemp_PresionAceite: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorTemp_PresionAceite: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en IndicadorTemp_PresionAceite: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var oilData = (OilData)data.dwData[0];
            Console.WriteLine($"Presión del aceite del motor: {oilData.EngOilPressure} psi");
            Console.WriteLine($"Temperatura del aceite del motor: {oilData.EngOilTemperature} °C");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos del IndicadorTemp_PresionAceite: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { OilData }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct OilData
    {
        public double EngOilPressure;
        public double EngOilTemperature;
    }
}

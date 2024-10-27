using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class IndicadorRPM
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            // Variables de RPM
            simconnect.AddToDataDefinition(DEFINITIONS.RPMData, "GENERAL ENG RPM:1", "RPM", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.RPMData, "ENG RPM ANIMATION PERCENT:1", "percent", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.RPMData, "GENERAL ENG PCT MAX RPM:1", "percent", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.RPMData, "MAX RATED ENGINE RPM", "RPM", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<RPMData>(DEFINITIONS.RPMData);
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.RPMData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorRPM: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorRPM: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en IndicadorRPM: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var rpmData = (RPMData)data.dwData[0];
            Console.WriteLine($"General Engine RPM: {rpmData.GeneralEngRPM} RPM");
            Console.WriteLine($"Engine RPM Animation Percent: {rpmData.EngRPMAnimationPercent} %");
            Console.WriteLine($"General Engine Percent Max RPM: {rpmData.GeneralEngPctMaxRPM} %");
            Console.WriteLine($"Max Rated Engine RPM: {rpmData.MaxRatedEngineRPM} RPM");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos del IndicadorRPM: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { RPMData }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct RPMData
    {
        public double GeneralEngRPM;
        public double EngRPMAnimationPercent;
        public double GeneralEngPctMaxRPM;
        public double MaxRatedEngineRPM;
    }
}
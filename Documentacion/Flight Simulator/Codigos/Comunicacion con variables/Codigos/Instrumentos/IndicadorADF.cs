using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class IndicadorADF
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("ADF Instrument", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            // ADF Active Frequency
            simconnect.AddToDataDefinition(DEFINITIONS.ADFData, "ADF ACTIVE FREQUENCY:1", "Hz", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            // ADF Radial Magnetic
            simconnect.AddToDataDefinition(DEFINITIONS.ADFData, "ADF RADIAL MAG:1", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // Register the ADF structure
            simconnect.RegisterDataDefineStruct<ADFData>(DEFINITIONS.ADFData);

            // Request data on the ADF from the simulator
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.ADFData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorADF: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorADF: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en IndicadorADF: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var adfData = (ADFData)data.dwData[0];
            int activeFrequency = adfData.ADFActiveFrequency;
            double radialMag = adfData.ADFRadialMag;

            Console.WriteLine($"ADF Active Frequency: {activeFrequency} Hz");
            Console.WriteLine($"ADF Radial Magnetic: {radialMag} degrees");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de IndicadorADF: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { ADFData }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct ADFData
    {
        public int ADFActiveFrequency;
        public double ADFRadialMag;
    }
}
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class IndicadorVor_GS
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV VOR LLAF64", "LLA", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV VOR DISTANCE", "nautical miles", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV RAW GLIDE SLOPE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV GS FLAG", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV GS LATLONALT", "latlonalt", SIMCONNECT_DATATYPE.LATLONALT, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorVor_GS: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorVor_GS: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en IndicadorVor_GS: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var navData = (Struct1)data.dwData[0];
            Console.WriteLine($"NAV VOR Distance: {navData.NavVorDistance} millas náuticas");
            Console.WriteLine($"NAV VOR Latitude: {navData.NavVorLatLonAlt.Latitude}");
            Console.WriteLine($"NAV VOR Longitude: {navData.NavVorLatLonAlt.Longitude}");
            Console.WriteLine($"NAV VOR Altitude: {navData.NavVorLatLonAlt.Altitude}");
            Console.WriteLine($"NAV Raw Glide Slope: {navData.NavRawGlideSlope} grados");
            Console.WriteLine($"NAV GS Flag: {navData.NavGsFlag}");
            Console.WriteLine($"NAV GS Latitude: {navData.NavGsLatLonAlt.Latitude}");
            Console.WriteLine($"NAV GS Longitude: {navData.NavGsLatLonAlt.Longitude}");
            Console.WriteLine($"NAV GS Altitude: {navData.NavGsLatLonAlt.Altitude}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de IndicadorVor_GS: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { Struct1 }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double NavVorLlaf64;
        public double NavVorDistance;
        public VORLatLonAlt NavVorLatLonAlt;
        public double NavRawGlideSlope;
        public int NavGsFlag;
        public GSLatLonAlt NavGsLatLonAlt;
        public double NavGsLlaf64;
        public double NavGlideSlopeLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct VORLatLonAlt
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct GSLatLonAlt
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }
}
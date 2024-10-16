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
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV VOR LATLONALT", "latlonalt", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV RAW GLIDE SLOPE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV GS FLAG", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV GS LATLONALT", "latlonalt", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV GS LLAF64", "LLA", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV GLIDE SLOPE LENGTH", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        var navData = (Struct1)data.dwData[0];
        Console.WriteLine($"NAV VOR LLAF64: {navData.NavVorLlaf64}");
        Console.WriteLine($"NAV VOR Distance: {navData.NavVorDistance} millas náuticas");
        Console.WriteLine($"NAV VOR LatLonAlt: {navData.NavVorLatLonAlt}");
        Console.WriteLine($"NAV Raw Glide Slope: {navData.NavRawGlideSlope} grados");
        Console.WriteLine($"NAV GS Flag: {navData.NavGsFlag}");
        Console.WriteLine($"NAV GS LatLonAlt: {navData.NavGsLatLonAlt}");
        Console.WriteLine($"NAV GS LLAF64: {navData.NavGsLlaf64}");
        Console.WriteLine($"NAV Glide Slope Length: {navData.NavGlideSlopeLength} metros");
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { Struct1 }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double NavVorLlaf64;
        public double NavVorDistance;
        public double NavVorLatLonAlt;
        public double NavRawGlideSlope;
        public int NavGsFlag;
        public double NavGsLatLonAlt;
        public double NavGsLlaf64;
        public double NavGlideSlopeLength;
    }
}


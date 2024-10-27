using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class ILS
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            // Localizer
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "NAV LOCALIZER", "frequency", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "NAV LOC AIRPORT IDENT", "string", SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "NAV LOC RUNWAY NUMBER", "number", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // Glide Slope
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "NAV RAW GLIDE SLOPE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "NAV GS FLAG", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "NAV GS LATLONALT", "latlonalt", SIMCONNECT_DATATYPE.LATLONALT, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // Markers
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "INNER MARKER", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "INNER MARKER LATLONALT", "latlonalt", SIMCONNECT_DATATYPE.LATLONALT, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "MIDDLE MARKER", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "MIDDLE MARKER LATLONALT", "latlonalt", SIMCONNECT_DATATYPE.LATLONALT, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "OUTER MARKER", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.ILSData, "OUTER MARKER LATLONALT", "latlonalt", SIMCONNECT_DATATYPE.LATLONALT, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // Register the ILS structure
            simconnect.RegisterDataDefineStruct<ILSData>(DEFINITIONS.ILSData);

            // Request data on the ILS from the simulator
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.ILSData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en ILS: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en ILS: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en ILS: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var ilsData = (ILSData)data.dwData[0];

            // Localizer
            double localizer = ilsData.NavLocalizer;
            string airportIdent = ilsData.NavLocAirportIdent;
            int runwayNumber = ilsData.NavLocRunwayNumber;

            Console.WriteLine($"NAV Localizer: {localizer} MHz");
            Console.WriteLine($"NAV Loc Airport Ident: {airportIdent}");
            Console.WriteLine($"NAV Loc Runway Number: {runwayNumber}");

            // Glide Slope
            Console.WriteLine($"NAV Raw Glide Slope: {ilsData.NavRawGlideSlope} grados");
            Console.WriteLine($"NAV GS Flag: {ilsData.NavGsFlag}");
            // Glide Slope
            Console.WriteLine($"NAV GS Latitude: {ilsData.NavGsLatLonAlt.Latitude}");
            Console.WriteLine($"NAV GS Longitude: {ilsData.NavGsLatLonAlt.Longitude}");
            Console.WriteLine($"NAV GS Altitude: {ilsData.NavGsLatLonAlt.Altitude}");

            // Markers
            Console.WriteLine($"Inner Marker: {ilsData.InnerMarker}");
            Console.WriteLine($"Inner Marker Latitude: {ilsData.InnerMarkerLatLonAlt.Latitude}");
            Console.WriteLine($"Inner Marker Longitude: {ilsData.InnerMarkerLatLonAlt.Longitude}");
            Console.WriteLine($"Inner Marker Altitude: {ilsData.InnerMarkerLatLonAlt.Altitude}");
            Console.WriteLine($"Middle Marker: {ilsData.MiddleMarker}");
            Console.WriteLine($"Middle Marker Latitude: {ilsData.MiddleMarkerLatLonAlt.Latitude}");
            Console.WriteLine($"Middle Marker Longitude: {ilsData.MiddleMarkerLatLonAlt.Longitude}");
            Console.WriteLine($"Middle Marker Altitude: {ilsData.MiddleMarkerLatLonAlt.Altitude}");
            Console.WriteLine($"Outer Marker: {ilsData.OuterMarker}");
            Console.WriteLine($"Outer Marker Latitude: {ilsData.OuterMarkerLatLonAlt.Latitude}");
            Console.WriteLine($"Outer Marker Longitude: {ilsData.OuterMarkerLatLonAlt.Longitude}");
            Console.WriteLine($"Outer Marker Altitude: {ilsData.OuterMarkerLatLonAlt.Altitude}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de ILS: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { ILSData }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct ILSData
    {
        public double NavLocalizer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string NavLocAirportIdent;
        public int NavLocRunwayNumber;

        // Glide Slope
        public double NavRawGlideSlope;
        public int NavGsFlag;
        public LatLonAlt NavGsLatLonAlt;

        // Markers
        public int InnerMarker;
        public LatLonAlt InnerMarkerLatLonAlt;
        public int MiddleMarker;
        public LatLonAlt MiddleMarkerLatLonAlt;
        public int OuterMarker;
        public LatLonAlt OuterMarkerLatLonAlt;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LatLonAlt
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }
}
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class IndicadorLOC
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("LOC Instrument", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            // NAV Localizer
            simconnect.AddToDataDefinition(DEFINITIONS.LOCData, "NAV LOCALIZER", "frequency", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            // NAV Loc Airport Ident
            simconnect.AddToDataDefinition(DEFINITIONS.LOCData, "NAV LOC AIRPORT IDENT", "string", SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            // NAV Loc Runway Number
            simconnect.AddToDataDefinition(DEFINITIONS.LOCData, "NAV LOC RUNWAY NUMBER", "number", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // Register the LOC structure
            simconnect.RegisterDataDefineStruct<LOCData>(DEFINITIONS.LOCData);

            // Request data on the LOC from the simulator
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.LOCData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorLOC: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorLOC: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en IndicadorLOC: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var locData = (LOCData)data.dwData[0];
            double localizer = locData.NavLocalizer;
            string airportIdent = locData.NavLocAirportIdent;
            int runwayNumber = locData.NavLocRunwayNumber;

            Console.WriteLine($"NAV Localizer: {localizer} MHz");
            Console.WriteLine($"NAV Loc Airport Ident: {airportIdent}");
            Console.WriteLine($"NAV Loc Runway Number: {runwayNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de IndicadorLOC: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { LOCData }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct LOCData
    {
        public double NavLocalizer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string NavLocAirportIdent;
        public int NavLocRunwayNumber;
    }
}

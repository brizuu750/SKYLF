using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class Program
{
    private static SimConnect simconnect = default!;
    private const int WM_USER_SIMCONNECT = 0x0402;

    static void Main(string[] args)
    {
        simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, WM_USER_SIMCONNECT, null, 0);

        simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(Simconnect_OnRecvSimobjectData);

        simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "INDICATED ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
        simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

        simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

        while (true)
        {
            simconnect.ReceiveMessage();
        }
    }

    private static void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        var altitudeData = (Struct1)data.dwData[0];
        double altitude = altitudeData.IndicatedAltitude;

        if (altitude > 2000) // Cambia 2000 por el valor que desees
        {
            Console.WriteLine("¡La altitud ha superado los 2000 pies!");
        }
        else {
            Console.WriteLine("¡Baja altitud!");
        }
    }

    enum DATA_REQUESTS
    {
        REQUEST_1
    }

    enum DEFINITIONS
    {
        Struct1
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double IndicatedAltitude;
    }
}

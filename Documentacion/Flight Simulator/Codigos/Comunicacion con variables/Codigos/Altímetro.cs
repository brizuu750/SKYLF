using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class Altimetro
{
    private static SimConnect simconnect = default!;
    private const int WM_USER_SIMCONNECT = 0x0402;

    static void Main(string[] args)
    {
        try
        {
            // Conectar con SimConnect
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, WM_USER_SIMCONNECT, null, 0);

            // Eventos de recepción de datos
            simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(Simconnect_OnRecvSimobjectData);

            // Definir las variables de altitud y Kollsman Setting HG
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "INDICATED ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "KOHLSMAN SETTING HG", "inHg", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // Registrar la estructura de datos
            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

            // Solicitar datos del objeto (avión del usuario)
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

            // Bucle principal
            while (true)
            {
                simconnect.ReceiveMessage();
            }
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    // Método para recibir y procesar datos
    private static void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        var altitudeData = (Struct1)data.dwData[0];
        double altitude = altitudeData.IndicatedAltitude;
        double kollsmanSettingHG = altitudeData.KollsmanSettingHG;

        // Mostrar datos en la terminal
        Console.WriteLine($"Altitud indicada: {altitude} pies");
        Console.WriteLine($"Ajuste Kollsman: {kollsmanSettingHG} inHg");

    }

    // Enum para solicitudes de datos
    enum DATA_REQUESTS
    {
        REQUEST_1
    }

    // Enum para definir estructuras de datos
    enum DEFINITIONS
    {
        Struct1
    }

    // Estructura para almacenar las variables
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double IndicatedAltitude;    // Altitud indicada
        public double KollsmanSettingHG;    // Ajuste Kollsman en inHg
    }
}

//Los siguientes codigos son los encargados de conectarse a los instrumentos del Flight simulator para leer los datos con el Simconnect, el api 

using Microsoft.FlightSimulator.SimConnect; //librerias para poder comunicarse con el simulador de vuelo 
using System;
using System.Runtime.InteropServices;

class IndicadorLOC //la clase que va a usarse en el codigo, el indicador loc en este caso
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()//se conecta al api
    {
        try
        {
            simconnect = new SimConnect("LOC Instrument", IntPtr.Zero, 0x0402, null, 0);/crea una nueva conexión con simconnect para comunicarse con el simulador de vuelo e indica que tipo de mensajes se manejara
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            simconnect.AddToDataDefinition(DEFINITIONS.LOCData, "NAV LOCALIZER", "frequency", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);//le dice al simulador que queremos recibir la variable localizer
            simconnect.AddToDataDefinition(DEFINITIONS.LOCData, "NAV LOC AIRPORT IDENT", "string", SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED); //le dice al simulador que queremos recibir la variable airportIdent
            simconnect.AddToDataDefinition(DEFINITIONS.LOCData, "NAV LOC RUNWAY NUMBER", "number", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);//le dice al simulador que queremos recibir la variable runwayNumber

            // Registra la estructura del LOC
            simconnect.RegisterDataDefineStruct<LOCData>(DEFINITIONS.LOCData);

            // Solicitar datos del LOC desde el simulador
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.LOCData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorLOC: " + ex.Message); //mensajes en caso de error de conexion
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorLOC: " + ex.Message);//mensaje en caso de error del indicador loc
        }
    }

    public void ReceiveMessage()//funcion para recibir los datos/mensajes del api
    {
        try
        {
            simconnect?.ReceiveMessage();
        }//recibe el mensaje del simulador
        catch (Exception ex)
        {
            Console.WriteLine("Error al recibir mensaje en IndicadorLOC: " + ex.Message); //en caso de error al recibir los datos muestra este mensaje
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

            Console.WriteLine($"NAV Localizer: {localizer} MHz"); //en caso de que la conexion sea exitosa muestra el dato de la variable localizer
            Console.WriteLine($"NAV Loc Airport Ident: {airportIdent}"); //en caso de que la conexion sea exitosa muestra el dato de la variable airoportident
            Console.WriteLine($"NAV Loc Runway Number: {runwayNumber}"); //en caso de que la conexion sea exitosa muestra el dato de la variable runwaynumber
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de IndicadorLOC: " + ex.Message);  //en caso de error al procesar los datos de la variable
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }//para etiquetar las solicitudes y las definiciones de datos, como nombres que ayudan a organizar la información en el código.
    enum DEFINITIONS { LOCData }//para etiquetar las solicitudes y las definiciones de datos, como nombres que ayudan a organizar la información en el código.

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct LOCData
    {
        public double NavLocalizer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string NavLocAirportIdent;
        public int NavLocRunwayNumber;
    }// los datos se almacenan en esta estructura para que el programa pueda usarlos.
}

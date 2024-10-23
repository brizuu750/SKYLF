//Los siguientes codigos son los encargados de conectarse a los instrumentos del Flight simulator para leer los datos con el Simconnect, el api 

using Microsoft.FlightSimulator.SimConnect; //librerias para poder comunicarse con el simulador de vuelo 
using System;
using System.Runtime.InteropServices;

class IndicadorVor_GS //la clase que va a usarse en el codigo, el indicador vor-gs en este caso
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect() //se conecta al api
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);  //crea una nueva conexión con simconnect para comunicarse con el simulador de vuelo e indica que tipo de mensajes se manejara
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV VOR LLAF64", "LLA", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED); //le dice al simulador que queremos recibir la variable navData.NavVorLatLonAlt.Latitude, navData.NavVorLatLonAlt.Altitude y navData.NavVorLatLonAlt.Longitude
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV VOR DISTANCE", "nautical miles", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED); //le dice al simulador que queremos recibir la variable navData.NavVorDistance
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV RAW GLIDE SLOPE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED); //le dice al simulador que queremos recibir la variable navData.NavRawGlideSlope
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV GS FLAG", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);//le dice al simulador que queremos recibir la variable navData.NavGsFlag 
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV GS LATLONALT", "latlonalt", SIMCONNECT_DATATYPE.LATLONALT, 0.0f, SimConnect.SIMCONNECT_UNUSED);//le dice al simulador que queremos recibir la variable navData.NavGsLatLonAlt.Latitude, navData.NavGsLatLonAlt.Longitude y navData.NavGsLatLonAlt.Altitude

            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0); 
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorVor_GS: " + ex.Message); //mensajes en caso de error de conexion
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorVor_GS: " + ex.Message); //mensaje en caso de error del indicador adf
        }
    }

    public void ReceiveMessage() //funcion para recibir los datos/mensajes del api
    {
        try
        {
            simconnect?.ReceiveMessage();
        } //recibe el mensaje del simulador
        catch (Exception ex)
        {
            Console.WriteLine("Error al recibir mensaje en IndicadorVor_GS: " + ex.Message);  //en caso de error al recibir los datos muestra este mensaje
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var navData = (Struct1)data.dwData[0];
            Console.WriteLine($"NAV VOR Distance: {navData.NavVorDistance} millas náuticas");  //en caso de que la conexion sea exitosa muestra el dato de la variable navdata.navvordistance
            Console.WriteLine($"NAV VOR Latitude: {navData.NavVorLatLonAlt.Latitude}"); //en caso de que la conexion sea exitosa muestra el dato de la variable navdata.navvorlatlonalt.latitude
            Console.WriteLine($"NAV VOR Longitude: {navData.NavVorLatLonAlt.Longitude}");//en caso de que la conexion sea exitosa muestra el dato de la variable navdata.navvorlatlonalt.longitude
            Console.WriteLine($"NAV VOR Altitude: {navData.NavVorLatLonAlt.Altitude}");//en caso de que la conexion sea exitosa muestra el dato de la variable navdata.navvorlatlonalt.altitude
            Console.WriteLine($"NAV Raw Glide Slope: {navData.NavRawGlideSlope} grados");//en caso de que la conexion sea exitosa muestra el dato de la variable navdata.navrawglideslope
            Console.WriteLine($"NAV GS Flag: {navData.NavGsFlag}");//en caso de que la conexion sea exitosa muestra el dato de la variable navData.NavGsflag
            Console.WriteLine($"NAV GS Latitude: {navData.NavGsLatLonAlt.Latitude}");//en caso de que la conexion sea exitosa muestra el dato de la variable navData.NavGsLatLonAlt.Latitude
            Console.WriteLine($"NAV GS Longitude: {navData.NavGsLatLonAlt.Longitude}");//en caso de que la conexion sea exitosa muestra el dato de la variable navData.NavGsLatLonAlt.Longitude
            Console.WriteLine($"NAV GS Altitude: {navData.NavGsLatLonAlt.Altitude}");//en caso de que la conexion sea exitosa muestra el dato de la variable navData.NavGsLatLonAlt.altitude
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de IndicadorVor_GS: " + ex.Message);  //en caso de error al procesar los datos de la variable
        }
    }

    enum DATA_REQUESTS { REQUEST_1 } //para etiquetar las solicitudes y las definiciones de datos, como nombres que ayudan a organizar la información en el código.
    enum DEFINITIONS { Struct1 } //para etiquetar las solicitudes y las definiciones de datos, como nombres que ayudan a organizar la información en el código.

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
    }// los datos se almacenan en esta estructura para que el programa pueda usarlos.

    [StructLayout(LayoutKind.Sequential)]
    struct VORLatLonAlt
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }// los datos se almacenan en esta estructura para que el programa pueda usarlos.

    [StructLayout(LayoutKind.Sequential)]
    struct GSLatLonAlt
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }// los datos se almacenan en esta estructura para que el programa pueda usarlos.
}

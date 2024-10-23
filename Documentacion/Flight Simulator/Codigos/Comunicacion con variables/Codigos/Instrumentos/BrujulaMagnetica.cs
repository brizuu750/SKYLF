//Los siguientes codigos son los encargados de conectarse a los instrumentos del Flight simulator para leer los datos con el Simconnect, el api 
using Microsoft.FlightSimulator.SimConnect; //librerias para poder comunicarse con el simulador de vuelo 
using System;
using System.Runtime.InteropServices;

class BrujulaMagnetica //la clase que va a usarse en el codigo, el altimetro en este caso
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect() //se conecta al api
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0); //crea una nueva conexión con simconnect para comunicarse con el simulador de vuelo e indica que tipo de mensajes se manejara
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "MAGNETIC COMPASS", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);//le dice al simulador que queremos recibir la variable "magnetic compass" 

            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1); //estructura de datos personalizada para manejar la información recibida

            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0); //envia solicitud al simulador para recibir los datos cada segundo
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en BrujulaMagnetica: " + ex.Message); //mensajes en caso de error de conexion
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en BrujulaMagnetica: " + ex.Message); //mensaje en caso de error en la burjula magnetica
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
            Console.WriteLine("Error al recibir mensaje en BrujulaMagnetica: " + ex.Message); //en caso de error al recibir los datos muestra este mensaje
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var compassData = (Struct1)data.dwData[0];
            Console.WriteLine($"Magnetic Compass: {compassData.MagneticCompass} grados");  //en caso de que la conexion sea exitosa muestra el dato de la variable magnetic compass
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de BrujulaMagnetica: " + ex.Message); //en caso de error al procesar los datos de la variable
        }
    }

    enum DATA_REQUESTS { REQUEST_1 } //para etiquetar las solicitudes y las definiciones de datos, como nombres que ayudan a organizar la información en el código.
    enum DEFINITIONS { Struct1 } //para etiquetar las solicitudes y las definiciones de datos, como nombres que ayudan a organizar la información en el código.

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double MagneticCompass;
    } // los datos se almacenan en esta estructura para que el programa pueda usarlos.
}

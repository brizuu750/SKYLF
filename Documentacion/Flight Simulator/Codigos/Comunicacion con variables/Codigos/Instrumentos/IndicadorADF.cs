//Los siguientes codigos son los encargados de conectarse a los instrumentos del Flight simulator para leer los datos con el Simconnect, el api 

using Microsoft.FlightSimulator.SimConnect; //librerias para poder comunicarse con el simulador de vuelo 
using System;
using System.Runtime.InteropServices;

class IndicadorADF //la clase que va a usarse en el codigo, el horizonte artificial en este caso
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()//se conecta al api
    {
        try
        {
            simconnect = new SimConnect("ADF Instrument", IntPtr.Zero, 0x0402, null, 0); //crea una nueva conexión con simconnect para comunicarse con el simulador de vuelo e indica que tipo de mensajes se manejara
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;
   
            simconnect.AddToDataDefinition(DEFINITIONS.ADFData, "ADF ACTIVE FREQUENCY:1", "Hz", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED); //le dice al simulador que queremos recibir la variable active frequency
            simconnect.AddToDataDefinition(DEFINITIONS.ADFData, "ADF RADIAL MAG:1", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED); //le dice al simulador que queremos recibir la variable radialmag

            // registre la estructura del ADF
            simconnect.RegisterDataDefineStruct<ADFData>(DEFINITIONS.ADFData);

            // Solicitar datos del ADF desde el simulador
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.ADFData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorADF: " + ex.Message); //mensajes en caso de error de conexion
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorADF: " + ex.Message); //mensaje en caso de error del indicador adf
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
            Console.WriteLine("Error al recibir mensaje en IndicadorADF: " + ex.Message); //en caso de error al recibir los datos muestra este mensaje
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var adfData = (ADFData)data.dwData[0];
            int activeFrequency = adfData.ADFActiveFrequency;
            double radialMag = adfData.ADFRadialMag;

            Console.WriteLine($"ADF Active Frequency: {activeFrequency} Hz");  //en caso de que la conexion sea exitosa muestra el dato de la variable activefrequency
            Console.WriteLine($"ADF Radial Magnetic: {radialMag} degrees");  //en caso de que la conexion sea exitosa muestra el dato de la variable radialmag
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de IndicadorADF: " + ex.Message);  //en caso de error al procesar los datos de la variable
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }  //para etiquetar las solicitudes y las definiciones de datos, como nombres que ayudan a organizar la información en el código.
    enum DEFINITIONS { ADFData }  //para etiquetar las solicitudes y las definiciones de datos, como nombres que ayudan a organizar la información en el código.

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct ADFData
    {
        public int ADFActiveFrequency;
        public double ADFRadialMag;
    } // los datos se almacenan en esta estructura para que el programa pueda usarlos.
}

using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class BrujulaMagnetica
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "MAGNETIC COMPASS", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en BrujulaMagnetica: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en BrujulaMagnetica: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en BrujulaMagnetica: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var compassData = (Struct1)data.dwData[0];
            Console.WriteLine($"Magnetic Compass: {compassData.MagneticCompass} grados");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de BrujulaMagnetica: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { Struct1 }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double MagneticCompass;
    }
}
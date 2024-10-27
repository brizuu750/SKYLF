using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class IndicadorCombustible
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            // Variables de combustible
            simconnect.AddToDataDefinition(DEFINITIONS.FuelData, "FUEL TOTAL CAPACITY", "gallons", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.FuelData, "FUEL TOTAL QUANTITY", "gallons", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.FuelData, "FUEL LEFT QUANTITY", "gallons", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.FuelData, "FUEL RIGHT QUANTITY", "gallons", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<FuelData>(DEFINITIONS.FuelData);
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.FuelData, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en IndicadorCombustible: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en IndicadorCombustible: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en IndicadorCombustible: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var fuelData = (FuelData)data.dwData[0];
            Console.WriteLine($"Capacidad total de combustible: {fuelData.TotalCapacity} galones");
            Console.WriteLine($"Cantidad total de combustible: {fuelData.TotalQuantity} galones");
            Console.WriteLine($"Cantidad de combustible en el tanque izquierdo: {fuelData.LeftQuantity} galones");
            Console.WriteLine($"Cantidad de combustible en el tanque derecho: {fuelData.RightQuantity} galones");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos del IndicadorCombustible: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { FuelData }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct FuelData
    {
        public double TotalCapacity;
        public double TotalQuantity;
        public double LeftQuantity;
        public double RightQuantity;
    }
}
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class Aceleraciones_Rotaciones_Velocidades
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("Flight Data Reader", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            // Yaw, Pitch y Roll/Bank
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "YAW STRING ANGLE", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "PLANE PITCH DEGREES", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "PLANE BANK DEGREES", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // Aceleraciones del avion
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ACCELERATION BODY X", "feet per second squared", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ACCELERATION BODY Y", "feet per second squared", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ACCELERATION BODY Z", "feet per second squared", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // Velocidades de rotacion
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ROTATION VELOCITY BODY X", "radians per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ROTATION VELOCITY BODY Y", "radians per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ROTATION VELOCITY BODY Z", "radians per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);
            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect con los movimientos: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en los movimientos: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje de los movimientos: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var flightData = (Struct1)data.dwData[0];

            // Yaw, Pitch, Roll/Bank
            Console.WriteLine($"YAW STRING ANGLE: {flightData.YawStringAngle} radianes");
            Console.WriteLine($"PLANE PITCH DEGREES: {flightData.PlanePitchDegrees} grados");
            Console.WriteLine($"PLANE BANK DEGREES: {flightData.PlaneBankDegrees} grados");

            // Aceleraciones
            Console.WriteLine($"ACCELERATION BODY X: {flightData.AccelerationBodyX} ft/s²");
            Console.WriteLine($"ACCELERATION BODY Y: {flightData.AccelerationBodyY} ft/s²");
            Console.WriteLine($"ACCELERATION BODY Z: {flightData.AccelerationBodyZ} ft/s²");

            // Velocidades de rotacion
            Console.WriteLine($"ROTATION VELOCITY BODY X: {flightData.RotationVelocityBodyX} rad/s");
            Console.WriteLine($"ROTATION VELOCITY BODY Y: {flightData.RotationVelocityBodyY} rad/s");
            Console.WriteLine($"ROTATION VELOCITY BODY Z: {flightData.RotationVelocityBodyZ} rad/s");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos de vuelo: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { Struct1 }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double YawStringAngle;
        public double PlanePitchDegrees;
        public double PlaneBankDegrees;

        public double AccelerationBodyX;
        public double AccelerationBodyY;
        public double AccelerationBodyZ;

        public double RotationVelocityBodyX;
        public double RotationVelocityBodyY;
        public double RotationVelocityBodyZ;
    }
}
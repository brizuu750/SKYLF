﻿using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class CoordinadorDeGiro
{
    private static SimConnect simconnect = default!;

    public void ConectarSimConnect()
    {
        try
        {
            simconnect = new SimConnect("SimvarWatcher", IntPtr.Zero, 0x0402, null, 0);
            simconnect.OnRecvSimobjectData += Simconnect_OnRecvSimobjectData;

            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "DELTA HEADING RATE", "degrees per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "TURN COORDINATOR BALL", "position", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "TURN INDICATOR RATE", "units", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

            simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }
        catch (COMException ex)
        {
            Console.WriteLine("Error al conectar SimConnect en CoordinadorDeGiro: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inesperado en CoordinadorDeGiro: " + ex.Message);
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
            Console.WriteLine("Error al recibir mensaje en CoordinadorDeGiro: " + ex.Message);
        }
    }

    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        try
        {
            var turnData = (Struct1)data.dwData[0];
            Console.WriteLine($"Delta Heading Rate: {turnData.DeltaHeadingRate} grados por segundo");
            Console.WriteLine($"Turn Coordinator Ball: {turnData.TurnCoordinatorBall}");
            Console.WriteLine($"Turn Indicator Rate: {turnData.TurnIndicatorRate} unidades");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al procesar datos del CoordinadorDeGiro: " + ex.Message);
        }
    }

    enum DATA_REQUESTS { REQUEST_1 }
    enum DEFINITIONS { Struct1 }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double DeltaHeadingRate;
        public double TurnCoordinatorBall;
        public double TurnIndicatorRate;
    }
}
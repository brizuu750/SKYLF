using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class Program
{
    private static SimConnect simconnect = default!;
    private const int WM_USER_SIMCONNECT = 0x0402;

    static void Main(string[] args)
    {
        Altimetro altimetro = new Altimetro();
        Velocimetro velocimetro = new Velocimetro();
        Variometro variometro = new Variometro();
        HorizonteArtificial horizonteartificial = new HorizonteArtificial();
        CoordinadorDeGiro coordinadordegiro = new CoordinadorDeGiro();
        IndicadorLOC indicadorloc = new IndicadorLOC();
        IndicadorVor_GS indicadorvor_gs = new IndicadorVor_GS();
        BrujulaMagnetica brujulamagnetica = new BrujulaMagnetica();

        altimetro.ConectarSimConnect();
        velocimetro.ConectarSimConnect();
        variometro.ConectarSimConnect();
        horizonteartificial.ConectarSimConnect();
        coordinadordegiro.ConectarSimConnect();
        indicadorloc.ConectarSimConnect();
        indicadorvor_gs.ConectarSimConnect();
        brujulamagnetica.ConectarSimConnect();

        // Bucle principal
        while (true)
        {
            simconnect.ReceiveMessage();
        }
    }
}
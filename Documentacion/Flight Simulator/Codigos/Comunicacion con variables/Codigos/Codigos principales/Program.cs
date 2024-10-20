using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;

class Program
{
    private const int WM_USER_SIMCONNECT = 0x0402;

    static void Main(string[] args)
    {
        Altimetro altimetro = new Altimetro();
        Velocimetro velocimetro = new Velocimetro();
        Variometro variometro = new Variometro();
        HorizonteArtificial horizonteartificial = new HorizonteArtificial();
        CoordinadorDeGiro coordinadorDeGiro = new CoordinadorDeGiro();
        BrujulaMagnetica brujulaMagnetica = new BrujulaMagnetica();

        IndicadorVor_GS indicadorVor_GS = new IndicadorVor_GS();
        IndicadorADF indicadorADF = new IndicadorADF();
        IndicadorLOC indicadorLOC = new IndicadorLOC();

        Aceleraciones_Rotaciones_Velocidades aceleraciones_Rotaciones_Velocidades = new Aceleraciones_Rotaciones_Velocidades();

        try
        {
            altimetro.ConectarSimConnect();
            velocimetro.ConectarSimConnect();
            variometro.ConectarSimConnect();
            horizonteartificial.ConectarSimConnect();
            coordinadorDeGiro.ConectarSimConnect();
            brujulaMagnetica.ConectarSimConnect();

            indicadorVor_GS.ConectarSimConnect();
            indicadorADF.ConectarSimConnect();
            indicadorLOC.ConectarSimConnect();

            aceleraciones_Rotaciones_Velocidades.ConectarSimConnect();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error durante la inicialización: " + ex.Message);
        }

        // Bucle principal
        while (true)
        {
            try
            {
                altimetro.ReceiveMessage();
                velocimetro.ReceiveMessage();
                variometro.ReceiveMessage();
                horizonteartificial.ReceiveMessage();
                coordinadorDeGiro.ReceiveMessage();
                brujulaMagnetica.ReceiveMessage();

                indicadorVor_GS.ReceiveMessage();
                indicadorADF.ReceiveMessage();
                indicadorLOC.ReceiveMessage();

                aceleraciones_Rotaciones_Velocidades.ReceiveMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error durante la recepción del mensaje: " + ex.Message);
            }
        }
    }
}

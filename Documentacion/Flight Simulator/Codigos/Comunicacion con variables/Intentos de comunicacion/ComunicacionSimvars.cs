using System;
using Microsoft.FlightSimulator.SimConnect;  // Asegúrate de haber referenciado SimConnect.dll
using System.Runtime.InteropServices;        // Para el manejo de estructuras
using System.Windows.Forms;                  // Para la aplicación de Windows Forms

public enum DEFINITIONS
{
    Struct1,
}

public enum DATA_REQUESTS
{
    REQUEST_1,
}

public class SimConnectExample : Form
{
    private SimConnect simconnect = null;
    private const int WM_USER_SIMCONNECT = 0x0402;  // ID de mensaje de SimConnect

    // Estructura para la variable de simulación
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct Struct1
    {
        public double airspeed;  // Velocidad del avión
    }

    public SimConnectExample()
    {
        // Crear conexión con Flight Simulator
        this.simconnect = new SimConnect("SimConnect Example", this.Handle, WM_USER_SIMCONNECT, null, 0);
        // Suscribir una SimVar (velocidad del avión en este caso)
        this.simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AIRSPEED INDICATED", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
        this.simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

        // Solicitar los datos de la simvar
        this.simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, 0, 0, 0, 0);
    }

    protected override void DefWndProc(ref Message m)
    {
        if (m.Msg == WM_USER_SIMCONNECT)
        {
            // Manejar los mensajes de SimConnect
            if (this.simconnect != null)
            {
                this.simconnect.ReceiveMessage();
            }
        }
        else
        {
            base.DefWndProc(ref m);
        }
    }

    // Método para recibir los datos
    private void Simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
    {
        switch ((DATA_REQUESTS)data.dwRequestID)
        {
            case DATA_REQUESTS.REQUEST_1:
                Struct1 result = (Struct1)data.dwData[0];
                Console.WriteLine($"Velocidad actual: {result.airspeed} knots");
                break;
        }
    }

    // Desconectar cuando se cierre la aplicación
    protected override void OnClosed(EventArgs e)
    {
        if (this.simconnect != null)
        {
            this.simconnect.Dispose();
            this.simconnect = null;
        }
        base.OnClosed(e);
    }

    [STAThread]
    static void Main()
    {
        Application.Run(new SimConnectExample());
    }
}
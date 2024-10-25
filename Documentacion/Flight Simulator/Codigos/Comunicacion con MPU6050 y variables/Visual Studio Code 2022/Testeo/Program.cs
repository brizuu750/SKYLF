using System;
using System.IO.Ports;

class Program
{
    static void Main(string[] args)
    {
        SerialPort serialPort = new SerialPort("COM6", 115200);
        try
        {
            serialPort.Open();
            Console.WriteLine("Puerto serial abierto.");

            while (true)
            {
                try
                {
                    if (serialPort.IsOpen)
                    {
                        string dataFromArduino = serialPort.ReadLine();
                        Console.WriteLine("Datos recibidos del MPU6050: " + dataFromArduino);
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Error: La lectura del puerto serial ha excedido el tiempo de espera.");
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine("Error de E/S: " + ioEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inesperado: " + ex.Message);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Error: Acceso no autorizado al puerto serial.");
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Error: Parámetro fuera de rango en la configuración del puerto serial.");
        }
        catch (IOException ioEx)
        {
            Console.WriteLine("Error de E/S: " + ioEx.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine("Puerto serial cerrado.");
            }
        }
    }
}

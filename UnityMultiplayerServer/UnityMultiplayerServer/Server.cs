using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

class Server
{
    private TcpListener tcpListener;
    private Thread listenerThread;

    private const int PORT = 8888;

    static void Main(string[] args)
    {
        Server server = new Server();
    }

    public Server()
    {
        this.tcpListener = new TcpListener(IPAddress.Any, PORT);
        this.listenerThread = new Thread(new ThreadStart(ListenForClients));
        this.listenerThread.Start();
    }

    static IPAddress GetIP()
    {
        // Get the host name of the current device
        string hostName = Dns.GetHostName();

        // Get the IP addresses associated with the host
        IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);

        return ipAddresses[3];
    }

    private void ListenForClients()
    {
        this.tcpListener.Start();
        Console.WriteLine($"Started Listining on http://{GetIP()}:{PORT}");

        while (true)
        {
            // blocks until a client has connected to the server
            TcpClient client = this.tcpListener.AcceptTcpClient();

            // create a thread to handle communication with the connected client
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
            clientThread.Start(client);
        }
    }

    private int fKeyPressCount = 0;

    private void HandleClientComm(object clientObj)
    {
        TcpClient tcpClient = (TcpClient)clientObj;
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] message = Encoding.ASCII.GetBytes("Welcome to the server!");
        clientStream.Write(message, 0, message.Length);

        while (true)
        {
            byte[] buffer = new byte[4096];
            int bytesRead = 0;

            try
            {
                bytesRead = clientStream.Read(buffer, 0, buffer.Length);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0)
                break;

            string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received data from client: " + receivedData);

            // Process the received data as needed

            // Check if the received data indicates an F key press
            if (receivedData == "F Key Pressed")
            {
                fKeyPressCount++;
                Console.WriteLine("F Key Press Count: " + fKeyPressCount);

                // Send the count back to the client
                byte[] countResponse = Encoding.ASCII.GetBytes($"F Key Press Count: {fKeyPressCount}");
                clientStream.Write(countResponse, 0, countResponse.Length);
            }
        }

        tcpClient.Close();
    }
}
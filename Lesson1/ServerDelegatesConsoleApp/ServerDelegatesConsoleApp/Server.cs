using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerDelegatesConsoleApp;

public class Server
{
    private Socket _socket;
    private readonly IPEndPoint _endpoint;

    private delegate void StartConnection(Socket socket);

    private delegate void SendResponse(Socket socket);

    public Server(string address, int port)
    {
        _endpoint = new IPEndPoint(IPAddress.Parse(address), port);
    }

    public void Start()
    {
        if (_socket != null) return;
        _socket = new Socket(SocketType.Stream, ProtocolType.IP);
        _socket.Bind(_endpoint);
        _socket.Listen(10);
        StartConnection startConnection = StartListen;
        startConnection.BeginInvoke(_socket, null, null);
    }

    public void Stop()
    {
        if (_socket == null) return;
        try
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _socket = null;
        }
        catch (SocketException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    private void StartListen(Socket socket)
    {
        while (true)
        {
            try
            {
                if (socket == null) continue;
                var clientSocket = socket.Accept();
                Console.WriteLine(clientSocket.RemoteEndPoint.ToString());
                SendResponse sendResponse = SendClientResponse;
                sendResponse.BeginInvoke(clientSocket, null, null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    private void SendClientResponse(Socket socket)
    {
        socket.Send(Encoding.ASCII.GetBytes(DateTime.Now.ToString()));
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}
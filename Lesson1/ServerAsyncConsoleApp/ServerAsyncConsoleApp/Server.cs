using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerAsyncConsoleApp;

public class Server
{
    private Socket _socket;
    private readonly IPEndPoint _endpoint;

    public Server(string address, int port)
    {
        _endpoint = new IPEndPoint(IPAddress.Parse(address), port);
    }

    public void Start()
    {
        if (_socket != null) return;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(_endpoint);
        _socket.Listen(10);
        _socket.BeginAccept(AcceptCallbackFunc, _socket);
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

    private void AcceptCallbackFunc(IAsyncResult result)
    {
        var socket = (Socket)result.AsyncState;
        var clientSocket = socket.EndAccept(result);
        Console.WriteLine(clientSocket.RemoteEndPoint.ToString());
        byte[] sendBuffer = Encoding.ASCII.GetBytes(DateTime.Now.ToString());
        clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, ClientCallback, clientSocket);

        socket.BeginAccept(AcceptCallbackFunc, socket);
    }

    private void ClientCallback(IAsyncResult result)
    {
        var socket = (Socket)result.AsyncState;
        socket.EndSend(result);
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}
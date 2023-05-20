using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerAsyncConsoleApp;

public class ServerAsync
{
    private Socket _socket;
    private readonly IPEndPoint _endpoint;

    public ServerAsync(string address, int port)
    {
        _endpoint = new IPEndPoint(IPAddress.Parse(address), port);
    }

    public async Task Start()
    {
        if (_socket != null) return;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(_endpoint);
        _socket.Listen(10);

        while (true)
        {
            var clientSocket = await _socket.AcceptAsync();
            Console.WriteLine(clientSocket.RemoteEndPoint.ToString());
            byte[] sendBuffer = Encoding.ASCII.GetBytes(DateTime.Now.ToString());
            await clientSocket.SendAsync(sendBuffer, SocketFlags.None);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
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
}
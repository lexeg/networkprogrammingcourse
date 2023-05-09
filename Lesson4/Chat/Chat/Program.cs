using System.Net;
using System.Net.Sockets;
using System.Text;

try
{
    var (remoteIpAddr, remotePort, localPort) = InitSettings();
    var thread = new Thread(ThreadReceive) { IsBackground = true };
    thread.Start(localPort);
    Console.ForegroundColor = ConsoleColor.Red;
    while (true)
    {
        SendData(Console.ReadLine(), remoteIpAddr, remotePort);
    }
}
catch (FormatException exception)
{
    Console.WriteLine($"Conversion impossible: {exception}");
}
catch (Exception exception)
{
    Console.WriteLine($"Exception: {exception.Message}");
}

(IPAddress ipAddress, int remotePort, int localPort) InitSettings()
{
    Console.SetWindowSize(40, 20);
    Console.Title = "Chat";
    Console.WriteLine("enter remote IP");
    var remoteIpAddr = IPAddress.Parse(Console.ReadLine() ?? throw new NullReferenceException("ip address is null"));
    Console.WriteLine("enter remote port");
    var remotePort = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("enter local port");
    var localPort = Convert.ToInt32(Console.ReadLine());
    return (remoteIpAddr, remotePort, localPort);
}

void ThreadReceive(object parameter)
{
    if (parameter is not int localPort) return;
    try
    {
        while (true)
        {
            var client = new UdpClient(localPort);
            IPEndPoint endPoint = null;
            var response = client.Receive(ref endPoint);
            var message = Encoding.Unicode.GetString(response);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Red;
            client.Close();
        }
    }
    catch (SocketException exception)
    {
        Console.WriteLine($"Socket exception: {exception.Message}");
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Exception: {exception.Message}");
    }
}

void SendData(string datagram, IPAddress ipAddress, int port)
{
    var client = new UdpClient();
    var endPoint = new IPEndPoint(ipAddress, port);
    try
    {
        var buffer = Encoding.Unicode.GetBytes(datagram);
        client.Send(buffer, buffer.Length, endPoint);
    }
    catch (SocketException exception)
    {
        Console.WriteLine($"Socket exception: {exception.Message}");
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Exception : {exception.Message}");
    }
    finally
    {
        client.Close();
    }
}
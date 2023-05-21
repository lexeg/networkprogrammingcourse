using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace MulticastClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        var thread = new Thread(Listener) { IsBackground = true };
        thread.Start();
    }

    private void Listener()
    {
        while (true)
        {
            var localAddress = new IPEndPoint(IPAddress.Any, 1025);
            var ipAddress = IPAddress.Parse("224.5.5.5");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(localAddress);
            //socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ipAddress, IPAddress.Any));
            //var endpoint = new IPEndPoint(ipAddress, 1025);
            //socket.Connect(endpoint);
            //socket.Send(Encoding.Default.GetBytes(_message));
            var buffer = new byte[1_024];
            socket.Receive(buffer);
            Dispatcher.Invoke(() => TextBox.Text = Encoding.Default.GetString(buffer));
            socket.Close();
        }
    }
    /*private void Listener()
    {
        using var udpClient = new UdpClient(1025);
        var multicastAddress = IPAddress.Parse("224.5.5.5");
        udpClient.JoinMulticastGroup(multicastAddress);
        while (true)
        {
            IPEndPoint endPoint = null;
            var result = udpClient.Receive(ref endPoint);
            var message = Encoding.Default.GetString(result);
            Dispatcher.Invoke(() => TextBox.Text = message);
        }
    }*/
}
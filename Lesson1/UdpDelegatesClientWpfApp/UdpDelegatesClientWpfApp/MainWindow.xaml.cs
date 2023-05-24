using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace UdpDelegatesClientWpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private System.Threading.Thread _thread;
    private Socket _socket;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartOnClick(object sender, RoutedEventArgs e)
    {
        if (_socket != null && _thread != null) return;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
        _socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 100));

        _thread = new System.Threading.Thread(ReceiveMessage);
        _thread.Start(_socket);
    }

    private void StopOnClick(object sender, RoutedEventArgs e)
    {
        if (_socket == null) return;
        //TODO: Переписать
        _thread.Abort();
        _thread = null;
        _socket.Shutdown(SocketShutdown.Receive);
        _socket.Close();
        _socket = null;
        MessagesTextBox.Text = "";
    }

    private void ClearOnClick(object sender, RoutedEventArgs e)
    {
        MessagesTextBox.Text = "";
    }

    private void SendOnClick(object sender, RoutedEventArgs e)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
        socket.SendTo(System.Text.Encoding.Unicode.GetBytes(MessageTextBox.Text),
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 100));
        socket.Shutdown(SocketShutdown.Send);
        socket.Close();
    }

    private void ReceiveMessage(object obj)
    {
        if (obj is not Socket receivedSocket)
        {
            return;
        }

        var buffer = new byte[1_024];
        do
        {
            EndPoint endPoint = new IPEndPoint(0x7F000000, 100);
            var length = receivedSocket.ReceiveFrom(buffer, ref endPoint);
            var clientIp = ((IPEndPoint)endPoint).Address.ToString();
            var receivedMessage = System.Text.Encoding.Unicode.GetString(buffer, 0, length);
            var text = $"\nReceived from {clientIp}\r\n{receivedMessage}\r\n";
            Dispatcher.Invoke(() => MessagesTextBox.Text += text);
        } while (true);
    }
}
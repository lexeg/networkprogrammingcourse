using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace UdpNetCoreAwaitWpfExample;

public partial class MainWindow
{
    private const int BufferSize = 1_024;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Socket _socket;

    private readonly EndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 1025);

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void StartOnClick(object sender, RoutedEventArgs e)
    {
        if (_socket != null) return;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1025));
        while (true)
        {
            var buffer = new byte[BufferSize];
            try
            {
                var receiveFromResult = await _socket.ReceiveFromAsync(buffer, SocketFlags.None, _clientEndPoint,
                    _cancellationTokenSource.Token);
                if (_socket == null) return;
                var length = receiveFromResult.ReceivedBytes;
                var address = ((IPEndPoint)receiveFromResult.RemoteEndPoint).Address.ToString();
                var message = Encoding.Unicode.GetString(buffer, 0, length);
                var text = $"\nПолучены данные от {address}\r\n{message}\n";

                Dispatcher.Invoke(() => MessagesTextBox.Text += text);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Остановили прослушивание");
                return;
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    private void StopOnClick(object sender, RoutedEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        _socket.Shutdown(SocketShutdown.Receive);
        _socket.Close();
        _socket = null;
        MessagesTextBox.Clear();
    }

    private void ClearOnClick(object sender, RoutedEventArgs e)
    {
        MessagesTextBox.Clear();
    }

    private async void SendOnClick(object sender, RoutedEventArgs e)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var buffer = Encoding.Unicode.GetBytes(MessageTextBox.Text);
        await _socket.SendToAsync(buffer, SocketFlags.None, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1025));
        socket.Shutdown(SocketShutdown.Send);
        socket.Close();
    }
}
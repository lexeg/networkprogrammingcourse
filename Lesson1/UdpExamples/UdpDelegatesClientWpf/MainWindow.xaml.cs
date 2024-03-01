using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace UdpDelegatesClientWpf;

public partial class MainWindow
{
    private readonly CancellationTokenSource _cancelTokenSource = new();
    private Thread _thread;
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

        _thread = new Thread(ReceiveMessage);
        _thread.Start(new ThreadData { Socket = _socket, CancellationToken = _cancelTokenSource.Token });
    }

    private void StopOnClick(object sender, RoutedEventArgs e)
    {
        if (_socket == null) return;
        _cancelTokenSource.Cancel();
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
        try
        {
            if (obj is not ThreadData threadData)
            {
                return;
            }

            var cancellationToken = threadData.CancellationToken;
            var socket = threadData.Socket;
            var buffer = new byte[1_024];
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                EndPoint endPoint = new IPEndPoint(0x7F000000, 100);
                var length = socket.ReceiveFrom(buffer, ref endPoint);
                var clientIp = ((IPEndPoint)endPoint).Address.ToString();
                var receivedMessage = System.Text.Encoding.Unicode.GetString(buffer, 0, length);
                var text = $"\nReceived from {clientIp}\r\n{receivedMessage}\r\n";
                Dispatcher.Invoke(() => MessagesTextBox.Text += text);
            } while (true);
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Operation was canceled.");
        }
    }

    private class ThreadData
    {
        public Socket Socket { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
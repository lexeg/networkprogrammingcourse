using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace UdpAsyncClientWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly StateObject _state = new StateObject();
        private Socket _socket;
        private IAsyncResult _receiveResult, _sendResult;

        private EndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 100);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartOnClick(object sender, RoutedEventArgs e)
        {
            if (_socket != null) return;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            _socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 100));

            _state.WorkSocket = _socket;
            _receiveResult = _socket.BeginReceiveFrom(_state.Buffer,
                0,
                StateObject.BufferSize,
                SocketFlags.None,
                ref _clientEndPoint,
                ReceiveCompleted, _state);
        }

        private void StopOnClick(object sender, RoutedEventArgs e)
        {
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
            var buffer = Encoding.Unicode.GetBytes(MessageTextBox.Text);
            _sendResult = socket.BeginSendTo(buffer, 0, buffer.Length,
                SocketFlags.None,
                new IPEndPoint(IPAddress.Parse("127.0.0.1"), 100),
                SendCompleted,
                socket);
        }

        private void SendCompleted(IAsyncResult ia)
        {
            if (!(ia.AsyncState is Socket socket))
            {
                return;
            }

            socket.EndSend(_sendResult);
            socket.Shutdown(SocketShutdown.Send);
            socket.Close();
        }

        private void ReceiveCompleted(IAsyncResult ia)
        {
            try
            {
                if (!(ia.AsyncState is StateObject stateObject))
                {
                    return;
                }

                var client = stateObject.WorkSocket;
                if (_socket == null) return;
                var length = client.EndReceiveFrom(_receiveResult, ref _clientEndPoint);

                var clientIpAddress = ((IPEndPoint)_clientEndPoint).Address.ToString();
                var receivedMessage = Encoding.Unicode.GetString(stateObject.Buffer, 0, length);
                var text = $"\nReceived from {clientIpAddress}\r\n{receivedMessage}\r\n";

                Dispatcher.Invoke(() => MessagesTextBox.Text += text);

                _receiveResult = _socket.BeginReceiveFrom(_state.Buffer,
                    0,
                    StateObject.BufferSize,
                    SocketFlags.None,
                    ref _clientEndPoint,
                    ReceiveCompleted,
                    _state);
            }
            catch (SocketException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private class StateObject
        {
            public Socket WorkSocket;
            public const int BufferSize = 1_024;
            public readonly byte[] Buffer = new byte[BufferSize];
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace MulticastServer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static string _message = "";
    private readonly Thread _thread = new(MulticastSend);

    public MainWindow()
    {
        InitializeComponent();
        _thread.IsBackground = true;
        _thread.Start();
    }

    private static void MulticastSend()
    {
        while (true)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            if (string.IsNullOrEmpty(_message)) continue;
            var destinationAddress = IPAddress.Parse("224.5.5.5");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 100);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(destinationAddress));
            var endpoint = new IPEndPoint(destinationAddress, 1025);
            socket.Connect(endpoint);
            socket.Send(Encoding.Default.GetBytes(_message));
            socket.Close();
        }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _message = TextBox.Text;
    }
}
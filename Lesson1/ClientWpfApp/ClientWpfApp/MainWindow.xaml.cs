using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace ClientWpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void LoadColumbiaEduPageClick(object sender, RoutedEventArgs e)
    {
        var url = new Uri("http://www.columbia.edu/~fdc/sample.html");
        var query = $@"GET {url.AbsoluteUri} HTTP/1.0
Host: {url.Host}
Connection: Close

";
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(url.Host, url.Port);
            socket.Send(Encoding.ASCII.GetBytes(query));
            var buffer = new byte[1024];
            int length;
            do
            {
                length = socket.Receive(buffer);
                TextBox1.Text += Encoding.ASCII.GetString(buffer, 0, length);
            } while (length > 0);
        }
        finally
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }

    private void ConnectToServerClick(object sender, RoutedEventArgs e)
    {
        var ip = IPAddress.Parse("127.0.0.1");
        var socket = new Socket(SocketType.Stream, ProtocolType.IP);
        try
        {
            socket.Connect(ip, 1_024);
            socket.Send(Encoding.ASCII.GetBytes($"Hello server. {DateTime.Now}"));
            socket.Shutdown(SocketShutdown.Send);
            var buffer = new byte[1_024];
            int length;
            do
            {
                length = socket.Receive(buffer);
                TextBox1.Text += Encoding.ASCII.GetString(buffer, 0, length);
            } while (length > 0);

            TextBox1.Text += Environment.NewLine;
        }
        finally
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
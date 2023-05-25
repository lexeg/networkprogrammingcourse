using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private TcpClient _client;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SendButtonOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(IpAddressTextBox.Text), Convert.ToInt32(PortTextBox.Text));
            _client = new TcpClient();
            _client.Connect(endPoint);

            var networkStream = _client.GetStream();
            var bytes = Encoding.Unicode.GetBytes(MessageTextBox.Text);
            networkStream.Write(bytes, 0, bytes.Length);
            _client.Close();
        }
        catch (SocketException socketException)
        {
            MessageBox.Show("Socket Exception:" + socketException.Message);
        }
        catch (Exception exception)
        {
            MessageBox.Show("Exception :" + exception.Message);
        }
    }

    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        _client?.Close();
    }
}
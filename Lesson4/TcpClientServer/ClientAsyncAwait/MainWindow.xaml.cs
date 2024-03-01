using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace ClientAsyncAwait;

public partial class MainWindow
{
    private TcpClient _tcpClient;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void SendButtonOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(IpAddressTextBox.Text), Convert.ToInt32(PortTextBox.Text));
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(endPoint);

            var networkStream = _tcpClient.GetStream();
            var bytes = Encoding.Unicode.GetBytes(MessageTextBox.Text);
            await networkStream.WriteAsync(bytes);
            _tcpClient.Close();
        }
        catch (SocketException ex)
        {
            MessageBox.Show($"Socket exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Exception: {ex.Message}");
        }
    }

    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        _tcpClient?.Close();
    }
}
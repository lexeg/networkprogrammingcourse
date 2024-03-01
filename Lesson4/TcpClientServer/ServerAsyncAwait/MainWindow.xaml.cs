using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServerAsyncAwait;

public partial class MainWindow
{
    private TcpListener _tcpListener;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void StartButtonOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _tcpListener = new TcpListener(IPAddress.Parse(IpAddressTextBox.Text), Convert.ToInt32(PortTextBox.Text));
            _tcpListener.Start();
            await Task.Run(ThreadReceive);
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
        _tcpListener.Stop();
    }

    private async Task ThreadReceive()
    {
        while (true)
        {
            if (!_tcpListener.Pending()) continue;
            var tcpClient = await _tcpListener.AcceptTcpClientAsync();
            var sr = new StreamReader(tcpClient.GetStream(), Encoding.Unicode);
            var message = await sr.ReadLineAsync() ?? string.Empty;
            Dispatcher.Invoke(() => MessagesListBox.Items.Add(message));
            tcpClient.Close();
            if (message.ToUpper() != "EXIT") continue;
            _tcpListener.Stop();
            Dispatcher.Invoke(Close);
        }
    }
}
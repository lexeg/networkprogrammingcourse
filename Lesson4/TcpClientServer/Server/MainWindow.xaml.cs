using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Server;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private TcpListener _tcpListener;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartButtonOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _tcpListener = new TcpListener(IPAddress.Parse(IpAddressTextBox.Text), Convert.ToInt32(PortTextBox.Text));
            _tcpListener.Start();
            var thread = new Thread(ThreadReceive) { IsBackground = true };
            thread.Start();
        }
        catch (SocketException socketException)
        {
            MessageBox.Show("Socket exception: " + socketException.Message);
        }
        catch (Exception exception)
        {
            MessageBox.Show("Exception : " + exception.Message);
        }
    }

    private void ThreadReceive()
    {
        while (true)
        {
            if (!_tcpListener.Pending()) continue;
            var tcpClient = _tcpListener.AcceptTcpClient();
            var streamReader = new StreamReader(tcpClient.GetStream(), Encoding.Unicode);
            var message = streamReader.ReadLine() ?? string.Empty;
            Dispatcher.Invoke(() => MessagesListBox.Items.Add(message));
            tcpClient.Close();
            if (message.ToUpper() != "EXIT") continue;
            _tcpListener.Stop();
            Dispatcher.Invoke(Close);
        }
    }

    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        _tcpListener?.Stop();
    }
}
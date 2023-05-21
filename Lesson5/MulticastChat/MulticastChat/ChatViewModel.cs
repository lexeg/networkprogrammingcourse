using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MulticastChat;

public class ChatViewModel : INotifyPropertyChanged
{
    private string _from = "";
    private string _message = "";

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Message> Messages { get; } = new();

    public string From
    {
        get => _from;
        set => SetProperty(ref _from, value, nameof(From));
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value, nameof(Message));
    }

    public AutoEventCommandBase SendCommand { get; }

    public ChatViewModel()
    {
        SendCommand = new AutoEventCommandBase(Send,
            _ => !string.IsNullOrEmpty(_from) && !string.IsNullOrEmpty(_message));
        _ = Listen();
    }

    private void Send(object o)
    {
        var destinationAddress = IPAddress.Parse("224.5.5.5");
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
            new MulticastOption(destinationAddress));
        var endPoint = new IPEndPoint(destinationAddress, 1025);
        socket.Connect(endPoint);
        socket.Send(Encoding.Unicode.GetBytes($"{_from}@@@{_message}"));
        socket.Close();
    }

    private async Task Listen()
    {
        while (true)
        {
            var localAddress = new IPEndPoint(IPAddress.Any, 1025);
            var ipAddress = IPAddress.Parse("224.5.5.5");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(localAddress);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(ipAddress, IPAddress.Any));
            var buffer = new byte[1024];
            await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

            var info = Encoding.Unicode.GetString(buffer).Split("@@@");

            if (info.Length != 2) continue;

            Messages.Add(new Message { From = info[0], Text = info[1] });
            socket.Close();
        }
    }

    private void SetProperty<T>(ref T oldValue, T newValue, string propertyName)
    {
        if (!(!oldValue?.Equals(newValue) ?? newValue != null)) return;
        oldValue = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace HttpClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var request = HttpWebRequest.CreateHttp(UrlTextBox.Text);
        request.Method = "GET";
        if (UseProxyCheckBox.IsChecked == true)
        {
            var proxy = new WebProxy(AddressProxy.Text);
            proxy.Credentials = new NetworkCredential(UserProxy.Text, PasswordProxy.Text);
            request.Proxy = proxy;
        }

        var response = (HttpWebResponse)request.GetResponse();
        if (response.StatusCode != HttpStatusCode.OK)
        {
            MessageBox.Show($"Запрос выполнился с ошибкой: {response.StatusCode}");
            return;
        }

        using var sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
        ResponseTextBox.Text = sr.ReadToEnd();
    }
}
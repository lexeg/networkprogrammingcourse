using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MailSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string serverAddress = "smtp.yandex.ru";
        private int serverPort = 587;//25;//587;//465;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AttachFile_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void NewLetter_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Send_OnClick(object sender, RoutedEventArgs e)
        {
            /*// create a message object
            MailMessage message = new MailMessage(FromBox.Text, ToBox.Text, ThemeBox.Text,BodyBox.Text);
            // create a send object
            SmtpClient client = new SmtpClient(serverAddress);
            client.Port = serverPort;//sets the server port
            // settings for sending mail
            //alexander.petrenko.develop@gmail.com
            //PerfectionIsNoLonger_28_ATrifle
            //ogkmysibhuvclgxg
            client.Credentials = new NetworkCredential("klim2041@yandex","PerfectionIsNoLonger_28_ATrifle");
            // call asynchronous message sending
            // client.SendAsync(message,"That's all");
            client.Send(message);*/
            MailAddress from = new MailAddress("klim2041@yandex", "Alexander Petrenko");
            MailAddress to = new MailAddress("alexander.petrenko.develop@gmail.com");
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Тест";
            m.Body = "Письмо-тест 2 работы smtp-клиента";
            SmtpClient smtp = new SmtpClient();//new SmtpClient(serverAddress, serverPort);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("klim2041@yandex","PerfectionIsNoLonger_28_ATrifle");
            // smtp.EnableSsl = true;
            // smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Host = serverAddress;
            smtp.Port = serverPort;
            smtp.Send(m);
            Console.WriteLine("Письмо отправлено");
        }
    }
}
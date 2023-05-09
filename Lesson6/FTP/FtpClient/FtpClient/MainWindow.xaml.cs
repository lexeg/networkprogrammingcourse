using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace FtpClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            //https://scientifically.info/publ/7-1-0-52
            HostNameBox.Text = "mirror.yandex.ru/";
            PortNumberBox.Text = "21";
            var address = $"ftp://{HostNameBox.Text}";

            var request = (FtpWebRequest)WebRequest.Create(address);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.UseBinary = true;
            request.EnableSsl = false;
            // request.UsePassive = true;
            if (AuthCheckBox.IsChecked ?? false)
            {
                request.Credentials = new NetworkCredential(UserNameBox.Text, PasswordBox.Text);
            }

            var response = (FtpWebResponse)request.GetResponse();
            using var sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
            var lst = new List<string>();
            while (!sr.EndOfStream)
            {
                lst.Add(sr.ReadLine());
            }

            var regex = new Regex(
                @"^([d-])([rwxt-]{3}){3}\s+\d{1,}\s+.*?(\d{1,})\s+(\w+\s+\d{1,2}\s+(?:\d{4})?)(\d{1,2}:\d{2})?\s+(.+?)\s?$",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase |
                RegexOptions.IgnorePatternWhitespace);
            var list = lst
                .Select(s =>
                {
                    var match = regex.Match(s);
                    if (match.Length > 5)
                    {
                        // Устанавливаем тип, чтобы отличить файл от папки (используется также для установки рисунка)
                        var type = match.Groups[1].Value == "d" ? "DIR.png" : "FILE.png";

                        // Размер задаем только для файлов, т.к. для папок возвращается
                        // размер ярлыка 4кб, а не самой папки
                        var size = "";
                        if (type == "FILE.png")
                            size = (Int32.Parse(match.Groups[3].Value.Trim()) / 1024).ToString() + " кБ";

                        return new FileDirectoryInfo(size, match.Groups[6].Value, match.Groups[4].Value, address);
                    }

                    return new FileDirectoryInfo();
                }).ToList();
            foreach (var fileDirectoryInfo in list)
            {
                FtpListView.Items.Add(fileDirectoryInfo);
            }
        }
    }
}
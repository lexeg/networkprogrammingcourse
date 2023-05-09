namespace FtpClient;

public class FileDirectoryInfo
{
    public string Address;

    public string FileSize { get; set; }

    public string Name { get; set; }

    public string Date { get; set; }

    public FileDirectoryInfo()
    {
    }

    public FileDirectoryInfo(string fileSize, string name, string date, string address)
    {
        FileSize = fileSize;
        Name = name;
        Date = date;
        Address = address;
    }
}
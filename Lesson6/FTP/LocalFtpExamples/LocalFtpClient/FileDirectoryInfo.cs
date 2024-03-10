using System;

namespace LocalFtpClient;

public class FileDirectoryInfo
{
    public FileTypes Type { get; set; }

    public string FileSize { get; set; }

    public string Name { get; set; }

    public DateTime Date { get; set; }
}
using ServerAsyncConsoleApp;

var server = new Server("127.0.0.1", 1024);
server.Start();
Console.Read();
server.Stop();
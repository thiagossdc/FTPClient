using System;
using System.IO;
using System.Net;

class FTPClient
{
    static void Main()
    {
        Console.WriteLine("Digite o endereço do servidor FTP (ex: ftp://exemplo.com):");
        string server = Console.ReadLine();

        Console.WriteLine("Digite o nome de usuário:");
        string username = Console.ReadLine();

        Console.WriteLine("Digite a senha:");
        string password = ReadPassword();

        Console.WriteLine("Escolha uma opção: /n1 - Listar arquivos/n2 - Upload/n3 - Download/n4 - Excluir arquivo");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ListFiles(server, username, password);
                break;
            case "2":
                Console.WriteLine("Digite o caminho do arquivo local para upload:");
                string uploadPath = Console.ReadLine();
                UploadFile(server, username, password, uploadPath);
                break;
            case "3":
                Console.WriteLine("Digite o nome do arquivo remoto para download:");
                string downloadFile = Console.ReadLine();
                DownloadFile(server, username, password, downloadFile);
                break;
            case "4":
                Console.WriteLine("Digite o nome do arquivo remoto para excluir:");
                string deleteFile = Console.ReadLine();
                DeleteFile(server, username, password, deleteFile);
                break;
            default:
                Console.WriteLine("Opção inválida!");
                break;
        }
    }

    static void ListFiles(string server, string username, string password)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(server);
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.Credentials = new NetworkCredential(username, password);

        using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        using StreamReader reader = new StreamReader(response.GetResponseStream());
        Console.WriteLine("Arquivos no servidor:");
        Console.WriteLine(reader.ReadToEnd());
    }

    static void UploadFile(string server, string username, string password, string filePath)
    {
        string fileName = Path.GetFileName(filePath);
        string uploadUrl = $"{server}/{fileName}";

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.Credentials = new NetworkCredential(username, password);

        using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using Stream requestStream = request.GetRequestStream();
        fileStream.CopyTo(requestStream);

        Console.WriteLine("Upload concluído!");
    }

    static void DownloadFile(string server, string username, string password, string fileName)
    {
        string downloadUrl = $"{server}/{fileName}";
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(downloadUrl);
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        request.Credentials = new NetworkCredential(username, password);

        using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        using Stream responseStream = response.GetResponseStream();
        using FileStream fileStream = new FileStream(fileName, FileMode.Create);
        responseStream.CopyTo(fileStream);

        Console.WriteLine("Download concluído!");
    }

    static void DeleteFile(string server, string username, string password, string fileName)
    {
        string deleteUrl = $"{server}/{fileName}";
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(deleteUrl);
        request.Method = WebRequestMethods.Ftp.DeleteFile;
        request.Credentials = new NetworkCredential(username, password);

        using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        Console.WriteLine("Arquivo excluído!");
    }

    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password;
    }
}

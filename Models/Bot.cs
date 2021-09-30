using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TelegramBotsApp.Models;

public class Bot 
{
    private string _name;
    private string _token;
    private string _description;
    private string _avatarPath;
    private int _updatingInterval;
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    public string Description { get => _description; set => _description = value; }
    public string AvatarPath { get => _avatarPath; set => _avatarPath = value; }
    public int UpdatingInterval { get => _updatingInterval; set => _updatingInterval = value; }

    public List<Command> commands;
    public Bot()
    {
        commands = new List<Command>();
        Description = "Описание бота (отправляется после сообщения /start от пользователя)";
    }
    public Bot(string name, string token, IEnumerable<Command> _commands)
    {
        Name = name;
        Token = token;
        commands = new List<Command>(_commands);
        Description = "Описание бота (отправляется после сообщения /start от пользователя)";
    }

    public async Task<bool> ConnectBot()
    {
        bool result;
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.telegram.org/bot" + Token + "/getMe");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine(response.StatusCode);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = true;
            }
            else
            {
                result = false;
            }
        }
        catch(WebException exc)
        {
            Debug.WriteLine(exc.Message);
            result = false;
        }
        return result;
    }

    public async Task<string> GetBotUsername()
    {
        string username = "";
        using (HttpClient httpClient = new HttpClient())
        {
            string botAboutResponse = await httpClient.GetStringAsync("https://api.telegram.org/bot" + Token + "/getMe");
            Console.WriteLine("got");
            JSONNode data = JSON.Parse(botAboutResponse);
            username = "@" + data["result"]["username"];
        }
        return username;
    }

    public async Task SendMessage(string message, int chat_id)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("chat_id", chat_id.ToString()));
            pairs.Add(new KeyValuePair<string, string>("text", message));

            FormUrlEncodedContent content = new FormUrlEncodedContent(pairs);
            await Task.Run(() => httpClient.PostAsync("https://api.telegram.org/bot" + Token + "/sendMessage", content));
        }
    }
    public async Task SendFile(int chat_id, string filePath)
    {
        filePath = filePath.Replace('\\', '/');
        MemoryStream memoryStream = new MemoryStream();
        FileStream fileStream = new FileStream(filePath, FileMode.Open);
        fileStream.CopyTo(memoryStream);
        byte[] fileBytes = memoryStream.ToArray();
        memoryStream.Close();
        fileStream.Close();
        HttpClient client = new HttpClient();
        using (var multipartContent = new MultipartFormDataContent())
        {
            StreamContent streamContent = new StreamContent(new MemoryStream(fileBytes));
            streamContent.Headers.Add("Content-Type", "application/octet-stream");
            streamContent.Headers.Add("Content-Disposition", $"form-data; name=\"document\"; filename=\"{filePath}\"");
            multipartContent.Add(streamContent, "document", $"{filePath}");
            var response = await client.PostAsync("https://api.telegram.org/bot" + Token + "/sendDocument?chat_id=" + chat_id, multipartContent);
        }
        client.Dispose();
    }

    public async Task SendPhoto(int chat_id, string imagePath)
    {
        imagePath = imagePath.Replace('\\', '/');
        Bitmap photo = new Bitmap(imagePath);
        ImageFormat format;
        switch (FileAttribute(imagePath))
        {
            case "png":
                format = ImageFormat.Png;
                break;
            case "jpg":
                format = ImageFormat.Jpeg;
                break;
            case "jpeg":
                format = ImageFormat.Jpeg;
                break;
            default:
                return;
        }
        MemoryStream memoryStream = new MemoryStream();
        photo.Save(memoryStream, format);
        byte[] imageBytes = memoryStream.ToArray();
        memoryStream.Close();
        HttpClient client = new HttpClient();
        using (var multipartContent = new MultipartFormDataContent())
        {
            StreamContent streamContent = new StreamContent(new MemoryStream(imageBytes));
            streamContent.Headers.Add("Content-Type", "application/octet-stream");
            streamContent.Headers.Add("Content-Disposition", $"form-data; name=\"photo\"; filename=\"{imagePath}\"");
            multipartContent.Add(streamContent, "photo", $"{chat_id + imagePath}");
            var response = await client.PostAsync("https://api.telegram.org/bot" + Token + "/sendPhoto?chat_id=" + chat_id, multipartContent);
        }
    }

    private string FileAttribute(string _filePath)
    {
        char[] attribute = new char[_filePath.Length];
        for (int i = _filePath.Length - 1, j = 0; i > 0; i--, j++)
        {
            attribute[j] = _filePath[i];
            if (_filePath[i] == '.')
            {
                Array.Reverse(attribute);
                return new string(attribute, _filePath.Length - j, j);
            }
        }
        return "";
    }
    public async void SendAvatar(int chat_id)
    {
        if (AvatarPath == null) return;
        await SendPhoto(chat_id, AvatarPath);
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TelegramBotsApp.Models
{
    public static class DataBase
    {
        private static StreamWriter safe;
        public static List<Bot> bots = new List<Bot>();
        public static int FindBotByToken(string _token)
        {
            for(int i = 0; i < bots.Count; i++)
            {
                if (bots[i].Token == _token) return i;
            }
            return -1;
        }
        public static bool Contain(Bot bot)
        {
            bool result = false;
            foreach (Bot _bot in bots)
            {
                if (_bot.Name == bot.Name || _bot.Token == bot.Token) result = true;
            }
            return result;
        }
        public static void Save()
        {
            string safeString = JsonConvert.SerializeObject(bots);
            File.Delete("safe");
            using(safe = new StreamWriter(new FileStream("safe", FileMode.OpenOrCreate)))
            {
                safe.WriteLine(safeString);
            }
        }
        public static void ExtractSafe()
        {
            DataBase.bots.Clear();
            using (StreamReader _safe = new StreamReader(new FileStream("safe", FileMode.OpenOrCreate)))
            {
                SimpleJSON.JSONNode safe = SimpleJSON.JSON.Parse(_safe.ReadToEnd());
                if (safe == null) return;
                foreach (SimpleJSON.JSONNode botSafe in safe.AsArray)
                {
                    List<Command> safeCommands = new List<Command>();
                    foreach(SimpleJSON.JSONNode command in botSafe["commands"].AsArray)
                    {
                        List<string> safeCommandFiles = new List<string>();
                        foreach (SimpleJSON.JSONNode file in command["files"].AsArray)
                        {
                            safeCommandFiles.Add(file);
                        }
                        safeCommands.Add(new Command(command["CommandText"], command["Answer"], safeCommandFiles));
                    }
                    bots.Add(new Bot
                    {
                        Name = botSafe["Name"],
                        Token = botSafe["Token"],
                        Description = botSafe["Description"],
                        AvatarPath = botSafe["AvatarPath"],
                        commands = safeCommands,
                        UpdatingInterval = botSafe["UpdatingInterval"] != null ? botSafe["UpdatingInterval"].AsInt : 800
                    });
                }
            }
        }
    }
}

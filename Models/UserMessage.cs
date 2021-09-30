using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotsApp.Models
{
    public struct UserMessage
    {
        private string text;
        private int id;
        public string Text { get => text; set => text = value; }
        public int ID { get => id; set => id = value; }
        public UserMessage(int _id, string _text)
        {
            id = _id;
            text = _text;
        }
    }
}

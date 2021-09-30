using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotsApp.Models
{
    public class Command
    {
        private string command;
        private string answer;
        public List<string> files;
        public string CommandText
        {
            get { return command; }
            set { command = value; }
        }
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }
        public Command()
        {
            files = new List<string>();
        }
        public Command(string _command, string _answer, IEnumerable<string> _files)
        {
            files = new List<string>(_files);
            CommandText = _command;
            Answer = _answer;
        }
    }
}

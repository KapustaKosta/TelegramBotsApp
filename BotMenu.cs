using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramBotsApp.Models;
using SimpleJSON;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using System.Collections;
using System.Net.Http.Headers;
using System.Threading;
using Newtonsoft.Json;

namespace TelegramBotsApp
{
    public partial class BotMenu : Form
    {
        public Dictionary<int, UserMessage> userCommands;
        public Dictionary<string, CommandMenu> openedCommandsMenus;
        private List<string> commandfiles;

        private int lastUpdateID = 0;
        private string token;
        private int updatingInterval = 800;

        private Bot bot
        {
            get
            {
                return DataBase.bots[DataBase.FindBotByToken(token)];
            }
            set
            {
                if(value != null) DataBase.bots[DataBase.FindBotByToken(token)] = value;
            }
        }

        #region Constructors
        public BotMenu()
        {
            InitializeComponent();
        }
        public BotMenu(Bot _bot)
        {
            InitializeComponent();
            botName.Text = _bot.Name;
            botToken.Text = _bot.Token;
            if(_bot.AvatarPath != null) pictureBox1.Image = Image.FromFile(_bot.AvatarPath);
            textBox3.Text = _bot.Description;
            token = _bot.Token;
            if (_bot.UpdatingInterval >= trackBar1.Minimum) trackBar1.Value = _bot.UpdatingInterval;
            if (_bot.UpdatingInterval >= trackBar1.Minimum) updatingIntervalLabel.Text = _bot.UpdatingInterval.ToString();
            userCommands = new Dictionary<int, UserMessage>();
            openedCommandsMenus = new Dictionary<string, CommandMenu>();
            commandfiles = new List<string>();
        }
        #endregion


        private void BotMenu_Load(object sender, EventArgs e)
        {
            ConnectBotAsync();
            UpdateCommandsList(default, default);
        }

        private async void ConnectBotAsync()
        {
            bool connectionResult = await Task.Run(() => bot.ConnectBot());
            if(connectionResult)
            {
                label4.Text = "Подключен";
                label4.ForeColor = Color.Lime;
                label3.Text = Task.Run(() => bot.GetBotUsername()).Result;
            }
            else 
            {
                label4.Text = "Не подключен";
                label4.ForeColor = Color.Tomato;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                FileItem fileItem = new FileItem(openDialog.FileName);
                commandfiles.Add(openDialog.FileName);
                flowLayoutPanel1.Controls.Add(fileItem);
                fileItem.button1.Click += (_sender, _e) =>
                {
                    flowLayoutPanel1.Controls.Remove(fileItem);
                    commandfiles.Remove(openDialog.FileName);
                };
            }
        }

        private string GetFileAttribute(string _filePath)
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

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Текст команды")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (IsEmpty(textBox1.Text))
            {
                textBox1.Text = "Текст команды";
                textBox1.ForeColor = Color.LightGray;
            }
        }

        private bool IsEmpty(string str)
        {
            foreach(char letter in str)
            {
                if (letter != ' ') return false;
            }
            return true;
        }

        private void AddCommand(string command, string answer)
        {
            if (!Contains(dataGridView1, command))
            {
                bot.commands.Add(new Command(command, answer, commandfiles));
                dataGridView1.Rows.Add(command);
                commandfiles.Clear();
                flowLayoutPanel1.Controls.Clear();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddCommand(textBox1.Text, textBox2.Text);
            textBox1.Text = "Текст команды";
            textBox2.Text = "Ответ";
            textBox1.ForeColor = Color.LightGray;
            textBox2.ForeColor = Color.LightGray;
        }

        private async void SendWelcomingMessage(int chat_id)
        {
            await bot.SendMessage(bot.Description, chat_id);
            await Task.Run(() => bot.SendAvatar(chat_id));
        }

        private bool Contains(DataGridView dataGrid, string text)
        {
            foreach(DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == text) return true;
            }
            return false;
        }

        private bool Contains(DataGridView dataGrid, string text, out int index)
        {
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == text)
                {
                    index = row.Index;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        private int FindByCommandText(IList<Command> commands, string text)
        {
            for(int i = 0; i < commands.Count(); i++)
            {
                if (commands[i].CommandText == text) return i;
            }
            return -1;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (bot.commands.Count == 0 || e.ColumnIndex != 0) return;

            int commandIndex = FindByCommandText(bot.commands,
                dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

            if(openedCommandsMenus.ContainsKey(bot.commands[commandIndex].CommandText))
            {
                openedCommandsMenus[bot.commands[commandIndex].CommandText].Close();
                openedCommandsMenus.Remove(bot.commands[commandIndex].CommandText);
                return;
            }
            CommandMenu commandMenu = new CommandMenu(token, bot.commands[commandIndex]);
            openedCommandsMenus.Add(bot.commands[commandIndex].CommandText, commandMenu);
            commandMenu.FormClosed += UpdateCommandsList;
            commandMenu.Show();
        }

        private void UpdateCommandsList(Object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            foreach(Command command in bot.commands)
            {
                dataGridView1.Rows.Add(command.CommandText);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || bot.commands.Count == 0 || dataGridView1.Rows[e.RowIndex].Cells[0].Value == null ||
                e.ColumnIndex != 1) return;
            int commandIndex = FindByCommandText(bot.commands,
                            dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
            if (openedCommandsMenus.ContainsKey(bot.commands[commandIndex].CommandText))
            {
                openedCommandsMenus[bot.commands[commandIndex].CommandText].Close();
                openedCommandsMenus[bot.commands[commandIndex].CommandText].FormClosed -= UpdateCommandsList;
                openedCommandsMenus.Remove(bot.commands[commandIndex].CommandText);
            }
            bot.commands.RemoveAt(commandIndex);
            dataGridView1.Rows.RemoveAt(e.RowIndex);
        }

        private void UpdateDescription(Object sender, EventArgs e)
        {
            botName.Text = bot.Name;
            textBox3.Text = bot.Description;
            if(bot.AvatarPath != null) pictureBox1.Image = new Bitmap(bot.AvatarPath);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            EditBotDescription editBotDescription = new EditBotDescription(token);
            editBotDescription.Show();
            editBotDescription.FormClosed += UpdateDescription;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Ответ")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (IsEmpty(textBox2.Text))
            {
                textBox2.Text = "Ответ";
                textBox2.ForeColor = Color.LightGray;
            }
        }

        private async void GetAndAnswerUpdates()
        {
            if (this.IsDisposed) return;
            await Task.Run(() => GetUpdates());
            await Task.Delay(updatingInterval);
            await Task.Run(() => AnswerLastUpdates());
            GetAndAnswerUpdates();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(label4.Text == "Подключен")
            {
                GetAndAnswerUpdates();
                timer1.Stop();
            }
        }

        private async Task AnswerLastUpdates()
        {
            foreach (int id in userCommands.Keys)
            {
                int commandID;
                if (Contains(dataGridView1, userCommands[id].Text, out commandID) && lastUpdateID != userCommands[id].ID)
                {
                    await bot.SendMessage(bot.commands[commandID].Answer, id);
                    if (bot.commands[commandID].files.Count != 0)
                    {
                        foreach (string filePath in bot.commands[commandID].files)
                        {
                            string fileAttribute = GetFileAttribute(filePath);
                            if (fileAttribute == "png" || fileAttribute == "jpg" || fileAttribute == "jpeg")
                            {
                                await bot.SendPhoto(id, filePath);
                                continue;
                            }
                            await bot.SendFile(id, filePath);
                        }
                    }
                    lastUpdateID = userCommands[id].ID;
                }
                if (userCommands[id].Text == "/start" && lastUpdateID != userCommands[id].ID)
                {
                    await Task.Run(() => SendWelcomingMessage(id));
                    lastUpdateID = userCommands[id].ID;
                }
            }
        }

        private async Task GetUpdates()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string response = await httpClient.GetStringAsync("https://api.telegram.org/bot" + token + "/getUpdates");
                    JSONNode data = JSON.Parse(response);
                    foreach (JSONNode resultNode in data["result"].AsArray)
                    {
                        int chatID = resultNode["message"]["chat"]["id"].AsInt;
                        if (userCommands.ContainsKey(chatID)) userCommands.Remove(chatID);
                        UserMessage message = new UserMessage(resultNode["update_id"].AsInt, (string)resultNode["message"]["text"]);
                        userCommands.Add(chatID, message);
                    }
                }
                catch (WebException exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            updatingInterval = trackBar1.Value;
            bot.UpdatingInterval = trackBar1.Value;
            updatingIntervalLabel.Text = trackBar1.Value.ToString();
        }
    }




}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramBotsApp.Models;

namespace TelegramBotsApp
{
    public partial class BotList : Form
    {

        public BotList()
        {
            InitializeComponent();
            DataBase.ExtractSafe();
            foreach (Bot bot in DataBase.bots)
            {
                dataGridView1.Rows.Add(bot.Name, bot.Token);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateBotAsync();
        }

        private async void CreateBotAsync()
        {
            CreateBotForm createBotForm = await Task.Run(() => new CreateBotForm());
            createBotForm.Show();
            createBotForm.createBot.Click += UpdateGridView;
            createBotForm.createBot.Click += (s, e) => DataBase.Save();
        }

        private void UpdateGridView(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            foreach (Bot bot in DataBase.bots)
            {
                dataGridView1.Rows.Add(bot.Name, bot.Token);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int clickedRowID = e.RowIndex;
            if (clickedRowID < 0 || clickedRowID > DataBase.bots.Count - 1) return;
            if (e.ColumnIndex == 2)
            {
                dataGridView1.Rows.RemoveAt(clickedRowID);
                DataBase.bots.RemoveAt(clickedRowID);
                DataBase.Save();
                return;
            }

            if (e.RowIndex <= DataBase.bots.Count && e.RowIndex > -1)
            {
                BotMenu botMenu = new BotMenu(DataBase.bots[clickedRowID]);
                botMenu.Show();
                botMenu.Activate();
                botMenu.FormClosed += (s, eventArgs) => DataBase.Save();
                botMenu.FormClosed += (s, eventArgs) => DataBase.ExtractSafe();
                botMenu.FormClosed += UpdateGridView;
                botMenu.FormClosed += (s, eventArgs) => botMenu.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }
    }
}

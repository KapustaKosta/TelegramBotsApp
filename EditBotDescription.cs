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
    public partial class EditBotDescription : Form
    {
        private string botToken;
        private int botIndex = -1;
        private string avatarPath;
        private Bot Bot
        {
            get { return DataBase.bots[botIndex]; }
            set { DataBase.bots[botIndex] = value; }
        }
        public EditBotDescription()
        {
            InitializeComponent();
        }

        public EditBotDescription(string _botToken)
        {
            InitializeComponent();
            botToken = _botToken;
            botIndex = DataBase.FindBotByToken(botToken);
            if (Bot.Name != null) textBox1.Text = Bot.Name;
            if (Bot.Description != null) textBox2.Text = Bot.Description;
            if (Bot.AvatarPath != null)
            {
                avatarPath = Bot.AvatarPath;
                pictureBox1.Image = Image.FromFile(Bot.AvatarPath);
                UpdateChangeAvatarButton();
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void EditBotDescription_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void UpdateChangeAvatarButton()
        {
            button1.Location = new Point(button1.Location.X + 107, button1.Location.Y);
            button1.Text = "Изменить аватар";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openDialog.FileName);
                avatarPath = openDialog.FileName;
                avatarPath = avatarPath.Replace('\\', '/');
                UpdateChangeAvatarButton();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int botIndex = DataBase.FindBotByToken(botToken);
            DataBase.bots[botIndex].Name = textBox1.Text;
            DataBase.bots[botIndex].Description = textBox2.Text;
            DataBase.bots[botIndex].AvatarPath = avatarPath;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramBotsApp.Models;

namespace TelegramBotsApp
{
    public partial class CommandMenu : Form
    {
        private string botToken;
        private Command command;
        private List<string> fileItems;
        public CommandMenu()
        {
            InitializeComponent();
            fileItems = new List<string>();
        }
        public CommandMenu(string _token, Command _command)
        {
            InitializeComponent();
            command = _command;
            botToken = _token;
            textBox1.Text = command.CommandText;
            textBox2.Text = command.Answer;
            fileItems = new List<string>(_command.files);
            foreach (string filePath in command.files)
            {
                FileItem fileItem = new FileItem(filePath);
                flowLayoutPanel1.Controls.Add(fileItem);
                fileItem.button1.Click += (_sender, _e) =>
                {
                    flowLayoutPanel1.Controls.Remove(fileItem);
                    fileItems.Remove(filePath);
                };
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            command.CommandText = textBox1.Text;
            command.Answer = textBox2.Text;
            command.files = fileItems;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void CommandMenu_Load(object sender, EventArgs e)
        {

        }

        private void addFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                FileItem fileItem = new FileItem(openDialog.FileName);
                fileItems.Add(openDialog.FileName);
                flowLayoutPanel1.Controls.Add(fileItem);
                fileItem.button1.Click += (_sender, _e) =>
                {
                    flowLayoutPanel1.Controls.Remove(fileItem);
                    fileItems.Remove(openDialog.FileName);
                };
            }
        }
    }
}

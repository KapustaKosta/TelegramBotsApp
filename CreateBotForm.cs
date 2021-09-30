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
    public partial class CreateBotForm : Form
    {
        public CreateBotForm()
        {
            InitializeComponent();
        }

        private void CreateBot_Click(object sender, EventArgs e)
        {
            Bot bot = new Bot { Name = textBox1.Text, Token = textBox2.Text };
            if (!DataBase.Contain(bot))
            {
                DataBase.bots.Add(new Bot { Name = textBox1.Text, Token = textBox2.Text });
                DataBase.Save();
                this.Close();
            }
            else { }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Имя")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }
        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Токен")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox1.Text == " ")
            {
                textBox1.Text = "Имя";
                textBox1.ForeColor = Color.LightGray;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "" || textBox2.Text == " ")
            {
                textBox2.Text = "Токен";
                textBox2.ForeColor = Color.LightGray;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CreateBotForm_Load(object sender, EventArgs e)
        {

        }
    }
}

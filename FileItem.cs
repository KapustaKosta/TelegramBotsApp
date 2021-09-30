using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelegramBotsApp
{
    public partial class FileItem : UserControl
    {
        private string _filePath;
        public FileItem()
        {
            InitializeComponent();
        }

        public FileItem(string filepath)
        {
            InitializeComponent();
            FilePath = FileTitle(filepath);
        }

        public string FilePath { get => _filePath; set { _filePath = value; label1.Text = value; } }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private string FileTitle(string _filePath)
        {
            char[] title = new char[_filePath.Length];
            for (int i = _filePath.Length - 1, j = 0; i > 0; i--, j++)
            {
                title[j] = _filePath[i];
                if (_filePath[i] == '\\')
                {
                    Array.Reverse(title);
                    return new string(title, _filePath.Length - j, j);
                }
            }
            return "";
        }
    }
}

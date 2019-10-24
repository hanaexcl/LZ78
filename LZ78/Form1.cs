using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LZ78 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e) {
            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    filePath = openFileDialog.FileName;
                }
            }

            if (filePath.Length == 0) return;

            string filecontext = string.Empty;
            byte[] tempContext = System.IO.File.ReadAllBytes(filePath);
            filecontext = BitConverter.ToString(tempContext);
            filecontext = filecontext.Replace("-", "");
            richTextBox1.Text = filecontext;
        }

        private void Button2_Click(object sender, EventArgs e) {
            string data = richTextBox1.Text;
            string tempStr = "";
            string encodeStr = "";
            int lastID = 0;
            int ID = 0;

            List<string> Dictionary = new List<string>();
            Dictionary.Add("");

            int nowid = 0;
            foreach (char str in data) {
                nowid += 1;

                tempStr += str;
                ID = Dictionary.FindIndex(tmp => tmp.Equals(tempStr));

                if (ID == -1) {
                    Dictionary.Add(tempStr);
                    //encodeStr += "(" + lastID + "," + tempStr[tempStr.Length - 1] + ")";
                    encodeStr += lastID + "," + tempStr[tempStr.Length - 1] + "|";
                    tempStr = "";
                    lastID = 0;
                } else {

                    if (nowid == data.Length ) {
                        encodeStr += ID + ",|";
                    }
                    lastID = ID;
                }
            }

            richTextBox2.Text = encodeStr;
            encodeStr = "";
            foreach (string str in Dictionary) {
                encodeStr += str + "\n";
            }
            richTextBox3.Text = encodeStr;
        }

        private void Button3_Click(object sender, EventArgs e) {
            List<string> Dictionary = new List<string>();
            Dictionary.AddRange(richTextBox3.Text.Split('\n'));
            string decodeStr = "";
            string tempStr;
            int tempID;

            foreach(string str in richTextBox2.Text.Split('|')) {
                if (str.Contains(',')) {
                    tempID = Convert.ToInt32(str.Split(',')[0]);
                    tempStr = str.Split(',')[1];

                    if (tempID == 0) {
                        decodeStr += tempStr;
                    } else {
                        decodeStr += Dictionary[tempID] + tempStr;
                    }
                    
                }
            }

            richTextBox1.Text = decodeStr;
        }
    }
}

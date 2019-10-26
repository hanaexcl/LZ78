using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LZ78 {
    public partial class Form1 : Form {
        byte[] tempContext;
        public Form1() {
            InitializeComponent();
        }

        /* str '11231112'
         * (11,+0)
         * (12,+1)
         * (23,+2)
         * (31,+3)
         * (111,+4)
         */

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
            tempContext = System.IO.File.ReadAllBytes(filePath);
            filecontext = BitConverter.ToString(tempContext);
            filecontext = filecontext.Replace("-", "");
            richTextBox1.Text = filecontext;
        }

        private void Button2_Click(object sender, EventArgs e) {
            string data = richTextBox1.Text;
            
            /*
             * 256 start
             * 257 end
             */
            
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            List<int> encode = new List<int>();

            for (int i = 0; i < 256; i++) dictionary.Add(((char)i).ToString(), i);

            encode.Add(256); // add start
            string w = "";
            foreach (char c in data) {
                string wc = w + c;
                if (dictionary.ContainsKey(wc)) {
                    w = wc;
                } else {
                    encode.Add(dictionary[w]);
                    dictionary.Add(wc, dictionary.Count+ 2);
                    w = c.ToString();
                }

                if (dictionary.Count >= ((1 << 14) - 1))
                {
                    encode.Add(256);
                    dictionary.Clear();
                    for (int i = 0; i < 256; i++) dictionary.Add(((char)i).ToString(), i);
                }
            }

            encode.Add(dictionary[w]);

            encode.Add(257); // end start

            richTextBox2.Text = string.Join(" ", encode.ToArray());


            string encodestring = "";
            
            foreach (KeyValuePair<string, int> item in dictionary) {
                encodestring += item.Key.Replace("\0", string.Empty) + '\n';
            }

            richTextBox3.Text = encodestring;

            double n1 = richTextBox1.Text.Length;
            double n2 = richTextBox2.Text.Length;
            double num = (n2 / n1);
            MessageBox.Show("壓縮比率：" + num.ToString("G", CultureInfo.InvariantCulture));
        }

        private void Button3_Click(object sender, EventArgs e) {
            //DateTime time_start = DateTime.Now;//計時開始 取得目前時間

            List<int> data = richTextBox2.Text.Split(' ').Select(Int32.Parse).ToList();


            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            string decode = null;
            string entry;
            string laststr = "";

            foreach (int c in data) {
                switch (c)
                {
                    case 256:
                        laststr = "";
                        dictionary.Clear();
                        for (int i = 0; i < 256; i++) dictionary.Add(i, ((char)i).ToString());
                        break;

                    case 257:
                        break;

                    default:
                        entry = "";

                        if (dictionary.ContainsKey(c))
                            entry = dictionary[c];
                        else
                            entry = laststr + laststr[0];


                        decode += entry;

                        if (!laststr.Length.Equals(0)) dictionary.Add(dictionary.Count + 2, laststr + entry[0]);
                        laststr = entry;
                        break;
                }
            }

            //DateTime time_end = DateTime.Now;

            //string result2 = ((TimeSpan)(time_end - time_start)).TotalMilliseconds.ToString();

            //MessageBox.Show(result2);
            richTextBox1.Text = decode;
        }

    }
}

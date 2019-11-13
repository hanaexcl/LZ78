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

        private void Button3_Click(object sender, EventArgs e)
        {
            List<int> data = richTextBox2.Text.Split(' ').Select(Int32.Parse).ToList();

            Dictionary<int, List<byte>> dictionary = new Dictionary<int, List<byte>>();

            List<byte> decode = new List<byte>();
            List<byte> lastbyt = new List<byte>();

            foreach (int c in data)
            {
                switch (c)
                {
                    case 256:

                        lastbyt.Clear();
                        dictionary.Clear();
                        for (int i = 0; i < 256; i++)
                        {
                            string tmpstr;
                            List<byte> tmpByt = new List<byte>();
                            if (i >= 65 && i <= 70)
                            {
                                tmpstr = ((char)i).ToString();
                                tmpByt.Add(byte.Parse(tmpstr, System.Globalization.NumberStyles.HexNumber));
                            }
                            if (i >= 48 && i <= 57)
                            {
                                tmpstr = ((char)i).ToString();
                                tmpByt.Add(byte.Parse(tmpstr, System.Globalization.NumberStyles.HexNumber));
                            }
                            dictionary.Add(i, tmpByt.ToList());
                            tmpByt.Clear();
                        }

                        break;

                    case 257:
                        break;

                    default:
                        List<byte> entry = new List<byte>();

                        if (dictionary.ContainsKey(c))
                        {
                            entry = dictionary[c].ToList();
                        }
                        else
                        {
                            entry = lastbyt.ToList();
                            if (lastbyt.Count > 0) entry.Add(lastbyt[0]);
                        }

                        decode.AddRange(entry.ToArray());

                        if (lastbyt.Count > 0)
                        {
                            List<byte> tmp = new List<byte>();
                            tmp = lastbyt.ToList();
                            tmp.Add(entry[0]);
                            dictionary.Add(dictionary.Count + 2, tmp.ToList());
                            tmp.Clear();
                        }

                        lastbyt = entry.ToList();
                        break;
                }
            }

            byte[] tmpAry = decode.ToArray();
            List<byte> decodeTxt = new List<byte>();
            for (int i = 0; i < tmpAry.Count() - 1; i+= 2)
            {
                decodeTxt.Add((byte)(tmpAry[i] * 16 + tmpAry[i + 1]));
            }

            System.IO.File.WriteAllBytes("C:\\Users\\user\\Desktop\\LZ78-master\\decode.txt", decodeTxt.ToArray());
            MessageBox.Show("ok");
        }
    }
}

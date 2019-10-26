using System;
using System.Collections.Generic;
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
        public static List<int> Compress(string uncompressed) {
            // build the dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(((char)i).ToString(), i);

            string w = string.Empty;
            List<int> compressed = new List<int>();

            foreach (char c in uncompressed) {
                string wc = w + c;
                if (dictionary.ContainsKey(wc)) {
                    w = wc;
                } else {
                    // write w to output
                    compressed.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }

            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);

            return compressed;
        }

        public static string Decompress(List<int> compressed) {
            // build the dictionary
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach (int k in compressed) {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];

                decompressed.Append(entry);

                // new sequence; add it to the dictionary
                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            return decompressed.ToString();
        }

        byte[] intToHexArray(int _num) {
            List<byte> _list = new List<byte>();
            //if (_num == 0) _list.Add(0x00);
            while (_num > 0) {
                _list.Add((byte)(_num & 0x00ff));
                _num >>= 8;
            }
            _list.Reverse();
            return _list.ToArray();
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

            /*
            List<int> compressed = Compress(data);
            richTextBox2.Text = string.Join(", ", compressed);
            string decompressed = Decompress(compressed);
            richTextBox3.Text = string.Join(", ", decompressed);
            */
            
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            List<int> encode = new List<int>();

            //for (int i = 0; i < 256; i++) {
            //    dictionary.Add(((char)i).ToString(), i);
            //}

            encode.Add(256); // add start
            string w = "";
            foreach (char c in data) {
                string wc = w + c;
                if (dictionary.ContainsKey(wc)) {
                    w = wc;
                } else {
                    encode.Add(dictionary[w]);
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }

            encode.Add(dictionary[w]);

            encode.Add(257); // end start

            //encodestring = "";

            //string encodestring = "";
            /*
            foreach (KeyValuePair<string, int> item in dictionary) {
                encodestring += item.Key.Replace("\0", string.Empty) + '\n';
            }

            richTextBox3.Text = encodestring;
            */

        }

        private void Button3_Click(object sender, EventArgs e) {
          
        }

        private void RichTextBox2_TextChanged(object sender, EventArgs e) {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace netugazou_v1
{
    public partial class Form1 : Form
    {
        static string fileName;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void 開くOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSV ファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.Cancel) return;

            fileName = ofd.FileName;
            //MessageBox.Show(fileName, "お知らせ"); //debug

            var list = loadCSV(fileName);
            //MessageBox.Show(list[1][0], "CPU温度");
            // list[0][0] = "TEMP"
            // list[1][0] = thermo_of_CPU
            textBox1.Text = list[1][0]; // set_CPU_Thermo_to_textbox1

            //var maxmax = list_Max(list);
            //var minmin = list_Min(list);

            //MessageBox.Show(maxmax.ToString(),"最大");
            //MessageBox.Show(minmin.ToString(), "最小");

            DrawThermoImage(list);


        }
        public void ColorScaleBCGYR(double input, ref int r, ref int g, ref int b)
        {
            //thermo color gen
            double tmp_input = Math.Cos(4 * Math.PI * input);
            int col_val = (int)((-tmp_input / 2 + 0.5) * 255);
            if (input >= (4.0 / 4.0)) { r = 255; g = 0; b = 0; }
            else if (input >= (3.0 / 4.0)) { r = 255; g = col_val; b = 0; }
            else if (input >= (2.0 / 4.0)) { r = col_val; g = 255; b = 0; }
            else if (input >= (1.0 / 4.0)) { r = 0; g = 255; b = col_val; }
            else if (input >= (0.0 / 4.0)) { r = 0; g = col_val; b = 255; }
            else { r = 0; g = 0; b = 255; }
        }
        public double list_Max(string[][] nums) //return_max_of_list
        {
            if (nums.Length == 0) return 0;
            double max = double.Parse(nums[2][0]);
            for (int i = 2; i < nums.Length; i++)
            {
                max = max > double.Parse(nums[i][0]) ? max : double.Parse(nums[i][0]);
                // Minの場合は不等号を逆にすればOK
            }
            return max;
        }
        public double list_Min(string[][] nums)//return_min_of_list
        {
            if (nums.Length == 0) return 0;
            double min = double.Parse(nums[2][0]);
            for (int i = 2; i < nums.Length; i++)
            {
                min = min < double.Parse(nums[i][0]) ? min : double.Parse(nums[i][0]);
                // Minの場合は不等号を逆にすればOK
            }
            return min;
        }
        private string[][] loadCSV(string filePath)// get_information_from_*.CSV
        {
            var list = new List<string[]>();
            StreamReader reader =
            new StreamReader(filePath, System.Text.Encoding.GetEncoding("Shift_JIS"));
            while (reader.Peek() >= 0)
            {
                list.Add(reader.ReadLine().Split(','));
            }
            reader.Close();
            return list.ToArray();
        }
        private void DrawThermoImage(string[][] list)// draw_Thermograph_to_picbox1
        {
            //Graphics objGrp = pictureBox1.CreateGraphics();
            pictureBox1.Image = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            Graphics objGrp = Graphics.FromImage(pictureBox1.Image);

            //objGrp.DrawLine(objPen, 20, 20, 200, 200);

            var maxmax = list_Max(list); textBox2.Text = maxmax.ToString();
            var minmin = list_Min(list); textBox3.Text = minmin.ToString();


            for (int i = 2; i < list.Length; i++)
            {
                double thermo_color = (double.Parse(list[i][0]) - minmin) / (maxmax - minmin);

                int ir = new int();
                int ig = new int();
                int ib = new int();
                ColorScaleBCGYR(thermo_color, ref ir, ref ig, ref ib);

                //MessageBox.Show(ir.ToString() + ig.ToString() + ib.ToString(), "RGB");
                //MessageBox.Show(i.ToString(), "cyc");

                SolidBrush bbb = new SolidBrush(Color.FromArgb(255, ir, ig, ib));


                objGrp.FillRectangle(bbb, new Rectangle(10 * ((i - 2) % 32), 10 * ((i - 2) / 32), 10, 10));


                bbb.Dispose();

            }
            objGrp.Dispose();

        }

        private void 名前をつけて保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg イメージ|*.jpg|Bitmap イメージ|*.bmp|Png イメージ|*.png";
            saveFileDialog1.Title = "画像の保存";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }

                fs.Close();
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (Path.GetExtension(files[0]) == ".csv")
            {
                fileName = files[0];
                var list = loadCSV(fileName);
                textBox1.Text = list[1][0];
                DrawThermoImage(list);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void バージョン情報AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialog1 dialog1 = new Dialog1();
            dialog1.ShowDialog();
        }
    }
    class Dialog1 : Form
    {
        Label label;
        LinkLabel linkLabel;
        Button okButton;
        Icon icon;
        public Dialog1()
        {
            // リソースアイコンを取得
            // (プロジェクト名 Project1，リソース名 Icon の場合)
            icon = netugazou_v1.Properties.Resources.icon;

            // 48 x 48 の大きさに修正
            icon = new Icon(icon, 72, 72);

            // アセンブリ情報を取得
            System.Reflection.AssemblyName assemblyName =
                System.Reflection.Assembly.GetExecutingAssembly().GetName();

            // プログラムのバージョンを文字列化
            string productVersion =
                assemblyName.Version.Major + "." +  // メジャーバージョン
                assemblyName.Version.Minor + "." +  // マイナーバージョン
                assemblyName.Version.Build;         // ビルド番号

            string project_name = "Netugazou ";
            string my_credit = "Copyright (c) 2022 ぬこむーちょ";

            // ラベルの設定
            label = new Label()
            {
                Location = new Point(90, 25),
                Size = new Size(190, 40),
                TextAlign = ContentAlignment.TopCenter,
                Text = project_name + productVersion + Environment.NewLine +
                    Environment.NewLine + my_credit,
            };

            string url = "http://www.gunjo-gentokan.com/";

            // リンクラベルの設定
            linkLabel = new LinkLabel()
            {
                Location = new Point(90, 75),
                Size = new Size(190, 20),
                TextAlign = ContentAlignment.TopCenter,
                Text = url,
            };
            linkLabel.Links.Add(0, url.Length, url);
            linkLabel.LinkClicked +=
                new LinkLabelLinkClickedEventHandler(linkLabel_LinkClicked);

            // ボタンの設定
            okButton = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(100, 110),
            };

            this.Controls.AddRange(new Control[]
            {
            okButton, label, linkLabel,
            });

            this.ClientSize = new Size(280, 150);
            this.Text = "バージョン情報";

            // ダイアログボックス用の設定
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            this.AcceptButton = okButton;
        }

        void linkLabel_LinkClicked(
            object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // (25, 25) の位置にアイコンを描画
            e.Graphics.DrawIcon(icon, 15, 25);
        }
    }
}

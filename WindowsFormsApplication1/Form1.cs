using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private String folder_path = null;
        private String copy_folder_path = null;
        private String folder_name = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        //元のファイル参照
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "フォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = @"C:\Windows";
            fbd.ShowNewFolderButton = true;

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                folder_path = fbd.SelectedPath;
                label1.Text = folder_path;
                folder_name = System.IO.Path.GetFileName(folder_path);
            }
        }

        //コピー先
        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "フォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = @"C:\Windows";
            fbd.ShowNewFolderButton = true;

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                copy_folder_path = fbd.SelectedPath;
                label2.Text = copy_folder_path;
            }
        }

        //開始
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;

            if (copy_folder_path == null || folder_path == null)
            {
                MessageBox.Show("フォルダーが指定されていません");
                return;
            }

            List<List<String>> main_file_array = new List<List<String>>();  //ファイル名一覧を取得
            List<String> folder_path_array = new List<String>();            //フォルダーパス一覧を取得
            List<String> folder_name_array = new List<String>();            //サブフォルダー名一覧

            //フォルダー一覧取得
            var main_dirs = Directory.GetDirectories(folder_path);

            foreach (var d in main_dirs)
            {
                String chas_str = d.Substring(d.Length - 1);
                folder_name_array.Add(chas_str);

                richTextBox1.Focus();
                richTextBox1.AppendText(d + "\n");
                folder_path_array.Add(d);
            }

            richTextBox1.Focus();
            richTextBox1.AppendText("=======================================" + "\n");

            //===========================================================================

            progressBar1.Minimum = 0;
            progressBar1.Maximum = folder_path_array.Count;
            progressBar1.Value = 0;

            //ファイル一覧取得
            for (int i = 0; i < folder_path_array.Count; i++)
            {
                List<String> file_list_array = new List<String>();

                progressBar1.Value++;

                var file_list = Directory.GetFiles(folder_path_array[i], "*");

                foreach (var d in file_list)
                {
                    richTextBox1.Focus();
                    richTextBox1.AppendText(d + "\n");

                    String chas_str = d.Replace(folder_path_array[i], "");
                    chas_str = chas_str.Remove(0, 1); ;
                    file_list_array.Add(chas_str);
                }
                main_file_array.Add(file_list_array);
            }

            //===========================================================================
            richTextBox1.Focus();
            richTextBox1.AppendText("=======================================" + "\n");

            //取得したファイル名を変換
            try
            {

                StreamReader sr = new StreamReader(@"./folder_list.csv");
                List<List<String>> name_base_array = new List<List<String>>();

                //ファイル名リストを取り出す
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    String[] values = line.Split(',');

                    List<String> lists = new List<String>();
                    lists.AddRange(values);

                    name_base_array.Add(lists);
                }
                
                for (int i = 0; i < name_base_array.Count; i++){
                    for(int c = 0; c < name_base_array[i].Count; c++)
                    {
                        richTextBox1.Focus();
                        richTextBox1.AppendText(name_base_array[i][c] + "\t");
                    }

                    richTextBox1.Focus();
                    richTextBox1.AppendText("\n");
                }

                richTextBox1.Focus();
                richTextBox1.AppendText("=======================================" + "\n");


                String dir_str = copy_folder_path + "\\" + folder_name;
                Directory.CreateDirectory(dir_str);

                //一致した文字を検索
                for (int i = 0; i < main_file_array.Count; i++)
                {
                    String chas_folder_str = dir_str + "\\" + folder_name_array[i];
                    String chas_copy_str = folder_path + "\\" + folder_name_array[i];
                    Directory.CreateDirectory(chas_folder_str);
                    
                    for (int c = 0; c < main_file_array[i].Count; c++)
                    {
                        String chas_file_str = main_file_array[i][c];

                        for(int x = 0;x < name_base_array.Count; x++)
                        {
                            if (folder_name_array[i].Contains(name_base_array[x][0]) && main_file_array[i][c].Contains(name_base_array[x][1]))
                            {
                                String rename_str = "";
                                if (name_base_array[x][3] == "") { 
                                    rename_str = name_base_array[x][2] + name_base_array[x][3] + name_base_array[x][4];
                                }
                                else
                                {
                                    rename_str = name_base_array[x][2] + "." + name_base_array[x][3] + name_base_array[x][4];
                                }

                                File.Copy(chas_copy_str + "\\" + main_file_array[i][c] , chas_folder_str + "\\" + rename_str , true);

                                richTextBox1.Focus();
                                richTextBox1.AppendText("ファイル名「" + main_file_array[i][c] + "」を「" + rename_str + "」に変更..." + "\n");
                            }
                        }
                    }
                }
                
                Console.WriteLine("OK!");

            }
            catch(IOException error){
                richTextBox1.Focus();
                richTextBox1.AppendText("Error!");
                MessageBox.Show("エラーが発生しました");
                Console.WriteLine(error);
            }

            richTextBox1.Focus();
            richTextBox1.AppendText("=======================================" + "\n");
            richTextBox1.Focus();
            richTextBox1.AppendText("ファイル名変更完了！" + "\n");

            MessageBox.Show("ファイル名変更完了!");


            //初期化
            button2.Enabled = true;
            folder_path = null;
            copy_folder_path = null;
            label1.Text = "ファイルを指定してください";
            label2.Text = "ファイルを指定してください";
        }


    }
}

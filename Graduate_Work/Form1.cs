using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace Graduate_Work
{
    public partial class Form1 : Form
    {
        public string informationFormString = "C:\\Users\\andre\\Desktop";

        private Form2 informationForm;

        private string fileName;
        private string path;

        private string menuStatus = "";

        public Form1()
        {
            InitializeComponent();

            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";

            //что-то где-то пошло не так...
            textBox1.SelectAll(); // Выделяем весь текст
            textBox1.SelectionLength = 0; // Сбрасываем выделение

            PictureBox pBox = new PictureBox();
            pBox.Location = new Point(482, 127);
            pBox.Width = 14;
            pBox.Height = 14;
            pBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pBox.Image = Image.FromFile("C:\\Users\\andre\\source\\repos\\Projects\\Graduate_Work\\Graduate_Work\\Resources\\refresh-button.png");
            panel1.Controls.Add(pBox);
            pBox.BringToFront();
            pBox.MouseClick += new MouseEventHandler(pictureBox1_Click);

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            if (informationForm == null || informationForm.IsDisposed)
            {
                informationForm = new Form2();
                informationForm.Show();
            }
        }

        private void DownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Для расшифровки и загрузки файла на локальный диск необходимо" +
              " выбрать интересующий Вас файл в структуре Яндекс Диска и нажать на кнопку 'Скачать'.";
            button1.Text = "Скачать";
            button1.Visible = true;
            treeView1.Nodes.Clear();
            menuStatus = "download";

            YandexDiskExplorer yandexDiskExplorer = new YandexDiskExplorer();
            treeView1.Nodes.Add(yandexDiskExplorer.GetYandexDiskStructure());
        }

        private void UploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Для шифрования и загрузки файла в облачное хранилище необходимо выбрать папку в структуре Яндекс Диска, " +
              "в которую будет загружен файл. После чего нажать на кнопку 'Загрузить'. Выбранный файл автоматически шифруется, затем " +
              "загружается в облачное хранилище.";
            button1.Text = "Загрузить";
            button1.Visible = true;
            menuStatus = "upload";

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (menuStatus == "download")
			{
                FolderBrowserDialog FBD = new FolderBrowserDialog();
                FBD.ShowNewFolderButton = false;
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    if (treeView1.SelectedNode != null)
                    {
                        // Выбранная нода
                        TreeNode selectedNode = treeView1.SelectedNode;
                        var fileName = selectedNode.Text;
                        var filePath = (selectedNode.Parent).FullPath;
                        filePath = filePath.Substring(1, filePath.Length-1);
                        filePath = filePath.Replace('\\', '/');
                        path = FBD.SelectedPath + '\\' + fileName;


                        //создание апи, метод скачивания
                        YandexAPI yandexAPI = new YandexAPI();
                        new Thread(() =>
                        {
                            yandexAPI.DownloadFile(yandexAPI.GetDownloadUrl(filePath, fileName), path);

                        }).Start();

                    }
                    else
                    {
                        MessageBox.Show("Не выбран файл для скачивания");
                    }
                }

                
            }
            else if (menuStatus == "upload")
			{
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
                    if (treeView1.SelectedNode != null /* && проверка что эта нода папка, а не файл*/)
					{
                        YandexAPI yandexAPI = new YandexAPI();
                        TreeNode selectedNode = treeView1.SelectedNode;
                        var YandexDir = selectedNode.FullPath;
                        YandexDir = YandexDir.Substring(1, YandexDir.Length - 1);
                        YandexDir = YandexDir.Replace('\\', '/');

                        // получаем выбранный файл
                        fileName = Path.GetFileName(openFileDialog1.FileName);
                        path = openFileDialog1.FileName;
                        // скриптяра.txt
                        // C:\Users\andre\Downloads\скриптяра.txt

                        new Thread(() =>
                        {
                            yandexAPI.UploadFile(yandexAPI.GetUploadUrl(YandexDir, fileName), path);

                        }).Start();
                    }
					else
					{
                        MessageBox.Show("Не выбрана папка для загрузки");
                    }
                   
                }

            }


        }

		private void pictureBox1_Click(object sender, EventArgs e)
		{
            treeView1.Nodes.Clear();

            YandexDiskExplorer yandexDiskExplorer = new YandexDiskExplorer();
            treeView1.Nodes.Add(yandexDiskExplorer.GetYandexDiskStructure());
        }


	}
}

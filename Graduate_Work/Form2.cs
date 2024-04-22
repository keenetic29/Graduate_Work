using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graduate_Work
{
  public partial class Form2 : Form
  {
    private static SaveFileDialog saveFileDialog;

    public Form2()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      DialogResult result = folderBrowserDialog.ShowDialog();

      if (result == DialogResult.OK)
      {
        comboBox1.Text = folderBrowserDialog.SelectedPath;
        comboBox1.Items.Add(comboBox1.Text);
      }
    }

    private void Form2_Load(object sender, EventArgs e)
    {

    }

    
  }
}

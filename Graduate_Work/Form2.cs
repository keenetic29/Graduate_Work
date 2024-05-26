using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graduate_Work
{
	public partial class Form2 : Form
	{
		private string filePath = "data.json";

		public Form2()
		{
			InitializeComponent();
			comboBox2.SelectedIndex = 2;
			LoadData();
		}


		private void Form2_Load(object sender, EventArgs e)
		{
			
		}

		private void SaveData() // Метод для сохранения данных
		{
			var data = new { 
				TextBoxToken = textBox2.Text,
				TextBoxKey = textBox1.Text,
				numericUpDownRounds = numericUpDown1.Value,
				comboBox2W = comboBox2.SelectedIndex,
				comboBox2Value = comboBox2.SelectedItem
			}; 
			File.WriteAllText(filePath, JsonConvert.SerializeObject(data)); // Сохраните данные в файл
		}

		private void LoadData() // Метод для загрузки данных
		{
			if (File.Exists(filePath))
			{
				var data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));
				textBox2.Text = data.TextBoxToken?.ToString() ?? "";
				textBox1.Text = data.TextBoxKey?.ToString() ?? "default key";
				numericUpDown1.Value = data.numericUpDownRounds.ToObject<int>() ?? 0;
				comboBox2.SelectedIndex = data.comboBox2W ?? 0;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			SaveData();
		}
	}
}

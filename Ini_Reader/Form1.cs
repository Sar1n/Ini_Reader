using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ini_Reader
{
    public partial class Form1 : Form
    {
		int flowHeight = 5, flowWidth = 5; //позиционирование надписей на форме
		string prev = ""; //предыдущее значение ключа

		public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //открытие файла по пути
        {
		   if (textBox1.Text != "")
			{
				panel1.Controls.Clear();
				flowWidth = 5;
				Display(textBox1.Text);
			}
		   else MessageBox.Show("Please, insert the path");
		}

		private void button2_Click(object sender, EventArgs e) //открытие файла дилоговым окном
		{
			try
			{ 
				var fileDialog = new OpenFileDialog();
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					panel1.Controls.Clear();
					flowWidth = 5;
					Display(fileDialog.FileName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		void CreateSection(string section) //вывод секции на форму
		{
			flowHeight = 5;
			Label sectionlabel = new Label();
			sectionlabel.Text = "[" + section + "]";
			sectionlabel.Location = new Point(flowHeight, flowWidth);
			panel1.Controls.Add(sectionlabel);
			flowWidth += 30;
		}
		void CreateNameValue(string key, string value) //вывод пары ключ - значение
		{
			flowHeight += 20; //отступ
			TextBox valueblock = new TextBox();
			if (key == prev) //если у данного значения ключ как у предыдущего
			{
				flowWidth -= 30;
				flowHeight += 130;

				valueblock.Text = value;
				valueblock.Size = new Size(120, 25);
				valueblock.Location = new Point(flowHeight, flowWidth);
				panel1.Controls.Add(valueblock);
			}
			else
			{
				prev = key;

				flowHeight = 5;					//вывод ключа 
				TextBox keyblock = new TextBox();
				keyblock.Text = key;
				keyblock.Size = new Size(120, 25);
				keyblock.Location = new Point(flowHeight, flowWidth);
				panel1.Controls.Add(keyblock);
				flowHeight += 130;

				Label eq = new Label();			//вывод =
				eq.Text = "=";
				eq.Size = new Size(15, 13);
				eq.Location = new Point(flowHeight, flowWidth);
				panel1.Controls.Add(eq);
				flowHeight += 25;

				valueblock.Text = value;		//вывод значения
				valueblock.Size = new Size(120, 25);
				valueblock.Location = new Point(flowHeight, flowWidth);
				panel1.Controls.Add(valueblock);
			}
			flowWidth += 30;
		}

		string TrimComment(string s) //вырезаем комментарий из строки
		{
			if (s.Contains(";"))
			{
				int index = s.IndexOf(';');
				s = s.Substring(0, index).Trim();
			}
			return s;
		}

		private void Display(string path) //метод чтения из файла
		{
			try
			{
				StreamReader reader = new StreamReader(path);
				string line;

				while ((line = reader.ReadLine()) != null)
				{
					line = TrimComment(line);

					if (line.Length == 0) continue;

					if (line.StartsWith("[") && line.Contains("]"))  //section
					{
						int index = line.IndexOf(']');
						string sectionName = line.Substring(1, index - 1).Trim();
						CreateSection(sectionName);
						continue;
					}

					if (line.Contains("="))  //key = value
					{
						int index = line.IndexOf('=');
						string key = line.Substring(0, index).Trim();
						string value = "";
						if (!line.Contains(",")) //если значиение одно
						{
							value = line.Substring(index + 1).Trim();
							CreateNameValue(key, value);
						}
						else //если значений несколько
						{
							line = line.Substring(index + 1).Trim();
							int indexcomma = 0;
							while (line.Contains(","))
							{
								indexcomma = line.IndexOf(",");
								value = line.Substring(0, indexcomma).Trim();
								line = line.Substring(indexcomma + 1).Trim();
								CreateNameValue(key, value);
							}
							value = line.Trim();
							CreateNameValue(key, value);
						}
					}
				}
				reader.Close();
			}
			catch (ArgumentException)
			{
				MessageBox.Show("Please, insert correct path");
			}
			catch (FileNotFoundException)
			{
				MessageBox.Show("File not found");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

	}
}

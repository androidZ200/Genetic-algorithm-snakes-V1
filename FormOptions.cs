using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace генетический_алгоритм__змейка___версия_1_
{
    public partial class FormOptions : Form
    {
        FormMain f;
        public FormOptions(FormMain form)
        {
            InitializeComponent();
            this.f = form;
            textBox1.Text = Convert.ToString(form.NextField.X);
            textBox2.Text = Convert.ToString(form.NextField.Y);
            AutoSaveCheckBox.Checked = form.AutoSave;
        }
        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string fileName = openFileDialog1.FileName;
            string open = System.IO.File.ReadAllText(fileName);
            f.FileName = fileName;

            string[][] splitOpen = new string[3][];
            splitOpen[0] = new string[1];
            splitOpen[1] = new string[1];
            splitOpen[2] = new string[open.Split('q')[2].Split('y').Length];
            splitOpen[0][0] = open.Split('q')[0];
            splitOpen[1][0] = open.Split('q')[1];
            for (int i = 0; i < splitOpen[2].Length; i++)
                splitOpen[2][i] = open.Split('q')[2].Split('y')[i];

            f.Generation = Convert.ToInt32(splitOpen[0][0].Split(' ')[0]);
            f.StopPoint = Convert.ToInt32(splitOpen[0][0].Split(' ')[1]);
            f.PlayingField = new Field[Convert.ToInt32(splitOpen[0][0].Split(' ')[2]),
                Convert.ToInt32(splitOpen[0][0].Split(' ')[3])];
            f.NextField = new Point(f.PlayingField.GetLength(0), f.PlayingField.GetLength(1));
            textBox1.Text = Convert.ToString(f.NextField.X);
            textBox2.Text = Convert.ToString(f.NextField.Y);

            for (int j = 0; j < f.PlayingField.GetLength(1); j++)
                for (int i = 0; i < f.PlayingField.GetLength(0); i++)
                {
                    if (i == 0 || j == 0 || i == f.PlayingField.GetLength(0) - 1 ||
                            j == f.PlayingField.GetLength(1) - 1)
                        f.PlayingField[i, j] = Field.Snake;
                    else
                    {
                        if (splitOpen[1][0].Split(' ')[(j - 1) * (f.PlayingField.GetLength(0) - 2) + i - 1] == "0")
                            f.PlayingField[i, j] = Field.Void;
                        else if (splitOpen[1][0].Split(' ')[(j - 1) * (f.PlayingField.GetLength(0) - 2) + i - 1] == "1")
                            f.PlayingField[i, j] = Field.Snake;
                        else if (splitOpen[1][0].Split(' ')[(j - 1) * (f.PlayingField.GetLength(0) - 2) + i - 1] == "2")
                            f.PlayingField[i, j] = Field.Eat;
                    }
                }

            f.Snakes.Clear();
            for (int i = 0; i < Convert.ToInt32(splitOpen[2].Length); i++)
            {
                int[,,] WallView = new int[6, 11, 3];
                int[,,] KindOfFood = new int[6, 11, 3];
                for (int i1 = 0; i1 < 6; i1++)
                    for (int i2 = 0; i2 < 11; i2++)
                        for (int i3 = 0; i3 < 3; i3++)
                        {
                            WallView[i1, i2, i3] = Convert.ToInt32(splitOpen[2][i].Split(' ')
                                [(i1 * 33 + i2 * 3 + i3) * 2]);
                            KindOfFood[i1, i2, i3] = Convert.ToInt32(splitOpen[2][i].Split(' ')
                                [(i1 * 33 + i2 * 3 + i3) * 2 + 1]);
                        }
                f.Snakes.Add(new Snake(WallView, KindOfFood, new Random()));
                f.Snakes[i].Energy = Convert.ToInt32(splitOpen[2][i].Split(' ')[396]);
                if (splitOpen[2][i].Split(' ')[397] == "Up") f.Snakes[i].Angle = Angle.Up;
                else if (splitOpen[2][i].Split(' ')[397] == "Down") f.Snakes[i].Angle = Angle.Down;
                else if (splitOpen[2][i].Split(' ')[397] == "Right") f.Snakes[i].Angle = Angle.Right;
                else if (splitOpen[2][i].Split(' ')[397] == "Left") f.Snakes[i].Angle = Angle.Left;
                f.Snakes[i].IsAlive = splitOpen[2][i].Split(' ')[398] == "True";
                for (int j = 399; j < splitOpen[2][i].Split(' ').Length; j += 2)
                    f.Snakes[i].TheBodyOfTheSnake.Add(new Point(Convert.ToInt32(splitOpen[2][i].Split(' ')[j]),
                        Convert.ToInt32(splitOpen[2][i].Split(' ')[j + 1])));
            }
            MessageBox.Show("файл открыт");
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            f.AutoSave = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string fileName = saveFileDialog1.FileName;
            f.FileName = fileName;
            f.save();
            f.AutoSave = AutoSaveCheckBox.Checked;
            MessageBox.Show("файл сохранен");
        }
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox1.Text) < 100)
                    f.NextField.X = 100;
                else if (Convert.ToInt32(textBox1.Text) > 1000)
                    f.NextField.X = 1000;
                else
                    f.NextField.X = Convert.ToInt32(textBox1.Text);
                if (Convert.ToInt32(textBox2.Text) < 60)
                    f.NextField.Y = 60;
                else if (Convert.ToInt32(textBox2.Text) > 600)
                    f.NextField.Y = 600;
                else
                    f.NextField.Y = Convert.ToInt32(textBox2.Text);
            }
            catch
            {
                MessageBox.Show("недопустимый параметр");
            }
            f.AutoSave = AutoSaveCheckBox.Checked;
            Close();
        }
        private void TeachButton_Click(object sender, EventArgs e)
        {
            f.TeachSnake = true;
            f.Stop = false;
            f.StartStopButton.Text = "Stop";
            try
            {
                f.t1.Start();
            }
            catch
            {
                f.t1.Abort();
                f.t1 = new Thread(f.Simulation);
                f.t1.Start();
            }
            Close();
        }
    }
}

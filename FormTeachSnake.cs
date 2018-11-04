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
    public partial class FormTeachSnake : Form
    {
        FormMain f;
        Field[,] field;
        public Angle angle = Angle.Down;
        public FormTeachSnake(FormMain f)
        {
            InitializeComponent();
            this.f = f;
        }
        private void FormTeachSnake_FormClosing(object sender, FormClosingEventArgs e)
        {
            angle = f.Snakes[f.SnakeManage].WhereToGo(field, new Random());
            Thread.Sleep(10);
            f.TeachSnake = false;
            f.form = null;
        }
        public void DrawView(Field[,] field)
        {
            this.field = field;
            if (pictureBox1.Height > 0 && pictureBox1.Width > 0)
            {
                if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
                Bitmap bmp = new Bitmap(field.GetLength(0) * 30, field.GetLength(1) * 30);
                Graphics g = Graphics.FromImage(bmp);
                for (int i = 0; i < field.GetLength(0); i++)
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        if (field[i, j] == Field.Void) g.FillRectangle(new SolidBrush(Color.LightGray), i * 30, j * 30, 29, 29);
                        else if (field[i, j] == Field.Snake) g.FillRectangle(new SolidBrush(Color.Black), i * 30, j * 30, 29, 29);
                        else g.FillRectangle(new SolidBrush(Color.DarkRed), i * 30, j * 30, 29, 29);
                    }
                pictureBox1.Image = bmp;
            }
            int Up = 0, Right = 0, Left = 0;
            for (int y = 0; y < 11; y++)
                for (int x = 0; x < 11; x++)
                    if (x != 5 || y != 5)
                        if (field[x, y] == Field.Eat)
                            if (x < 6)
                            {
                                Left += f.Snakes[f.SnakeManage].KindOfFood[x, y, 0];
                                Up += f.Snakes[f.SnakeManage].KindOfFood[x, y, 1];
                                Right += f.Snakes[f.SnakeManage].KindOfFood[x, y, 2];
                            }
                            else
                            {
                                Left += f.Snakes[f.SnakeManage].KindOfFood[10 - x, y, 2];
                                Up += f.Snakes[f.SnakeManage].KindOfFood[10 - x, y, 1];
                                Right += f.Snakes[f.SnakeManage].KindOfFood[10 - x, y, 0];
                            }
                        else if (field[x, y] == Field.Snake)
                            if (x < 6)
                            {
                                Left += f.Snakes[f.SnakeManage].WallView[x, y, 0];
                                Up += f.Snakes[f.SnakeManage].WallView[x, y, 1];
                                Right += f.Snakes[f.SnakeManage].WallView[x, y, 2];
                            }
                            else
                            {
                                Left += f.Snakes[f.SnakeManage].WallView[10 - x, y, 2];
                                Up += f.Snakes[f.SnakeManage].WallView[10 - x, y, 1];
                                Right += f.Snakes[f.SnakeManage].WallView[10 - x, y, 0];
                            }
            label1.Text = Convert.ToString(Left);
            label2.Text = Convert.ToString(Up);
            label3.Text = Convert.ToString(Right);
        }
        private void FormTeachSnake_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) angle = Angle.Up;
            else if (e.KeyCode == Keys.A) angle = Angle.Left;
            else if (e.KeyCode == Keys.D) angle = Angle.Right;
        }
    }
}

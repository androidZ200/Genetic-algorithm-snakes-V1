using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace генетический_алгоритм__змейка___версия_1_
{
    public partial class FormMain : Form
    {
        Random rand = new Random();
        public FormTeachSnake form = null;
        public Field[,] PlayingField = new Field[162, 92];
        public Point NextField = new Point(162, 92);
        public List<Snake> Snakes = new List<Snake>();
        public int StopPoint = 0;
        public int Generation = 0;
        public string FileName = null;
        public bool TeachSnake = false;
        public bool Stop = true;
        public bool AutoSave = false;
        public Thread t1;
        public int SnakeManage = 0;

        public void Simulation()
        {
            while (!Stop)
            {
                Invoke(new EventHandler(delegate { GenerationLabel.Text = Convert.ToString(Generation); }));
                Invoke(new EventHandler(delegate
                {
                    int k = 0;
                    for (int i = 0; i < Snakes.Count; i++)
                        if (Snakes[i].IsAlive) k++;
                    AllSnakesLabel.Text = Convert.ToString(k);
                }));
                Invoke(new EventHandler(delegate { CounterSnakeLabel.Text = Convert.ToString(StopPoint); }));
                if (StopPoint >= Snakes.Count) StopPoint = 0;
                Field[,] newField = new Field[11, 11];
                for (int x = 0, i = Snakes[StopPoint].TheBodyOfTheSnake[Snakes[StopPoint].TheBodyOfTheSnake.Count - 1].X - 5; x < 11; i++, x++)
                    for (int y = 0, j = Snakes[StopPoint].TheBodyOfTheSnake[Snakes[StopPoint].TheBodyOfTheSnake.Count - 1].Y - 5; y < 11; j++, y++)
                        try
                        {
                            newField[x, y] = PlayingField[i, j];
                        }
                        catch
                        {
                            newField[x, y] = Field.Void;
                        }
                Angle WhereToGo = Angle.Up;
                if (!TeachSnake || StopPoint != SnakeManage)
                    WhereToGo = Snakes[StopPoint].ChangeAngle(Snakes[StopPoint].WhereToGo(Snake.GetView(newField, Snakes[StopPoint].Angle), rand));
                else
                {
                    if (form == null)
                    {
                        form = new FormTeachSnake(this);
                        Invoke(new EventHandler(delegate { form.Show(); }));
                    }
                    Invoke(new EventHandler(delegate { form.DrawView(Snake.GetView(newField, Snakes[SnakeManage].Angle)); }));
                    Invoke(new EventHandler(delegate { form.Text = Convert.ToString(Snakes[SnakeManage].Energy); }));
                    while (form.angle == Angle.Down)
                        continue;
                    WhereToGo = form.angle;
                    WhereToGo = Snakes[SnakeManage].MakeYourMove(WhereToGo, Snake.GetView(newField, Snakes[SnakeManage].Angle), rand);
                    form.angle = Angle.Down;
                }
                int x1 = 0, y1 = 0;
                if (WhereToGo == Angle.Up) y1--;
                else if (WhereToGo == Angle.Down) y1++;
                else if (WhereToGo == Angle.Right) x1++;
                else x1--;
                Field timeField = PlayingField[Snakes[StopPoint].TheBodyOfTheSnake[Snakes[StopPoint].TheBodyOfTheSnake.Count - 1].X + x1,
                    Snakes[StopPoint].TheBodyOfTheSnake[Snakes[StopPoint].TheBodyOfTheSnake.Count - 1].Y + y1];
                if (timeField == Field.Eat)
                    while (true)
                    {
                        int X = rand.Next(1, PlayingField.GetLength(0) - 1);
                        int Y = rand.Next(1, PlayingField.GetLength(1) - 1);
                        if (PlayingField[X, Y] == Field.Void)
                        {
                            PlayingField[X, Y] = Field.Eat;
                            break;
                        }
                    }
                for (int i = 0; i < Snakes[StopPoint].TheBodyOfTheSnake.Count; i++)
                    PlayingField[Snakes[StopPoint].TheBodyOfTheSnake[i].X, Snakes[StopPoint].TheBodyOfTheSnake[i].Y] = Field.Void;
                Snakes[StopPoint].Go(timeField);
                for (int i = 0; i < Snakes[StopPoint].TheBodyOfTheSnake.Count; i++)
                    PlayingField[Snakes[StopPoint].TheBodyOfTheSnake[i].X, Snakes[StopPoint].TheBodyOfTheSnake[i].Y] = Field.Snake;
                bool isLive = false;
                for (int i = 0; i < Snakes.Count; i++) if (Snakes[i].IsAlive) isLive = true;

                if (isLive)
                    while (true)
                    {
                        if (StopPoint == SnakeManage && !Snakes[SnakeManage].IsAlive)
                        {
                            SnakeManage++;
                            SnakeManage %= Snakes.Count;
                        }
                        StopPoint++;
                        StopPoint %= Snakes.Count;
                        if (ShowCheckBox.Checked && StopPoint == 1)
                        {
                            Thread.Sleep(5);
                            DrawField();
                        }
                        if (Snakes[StopPoint].IsAlive) break;
                    }
                else
                {
                    Snake[] snakesLenght = new Snake[10];
                    for (int j = 0; j < 10; j++)
                    {
                        Snake snakeTime = Snakes[0];
                        for (int i = 1; i < Snakes.Count; i++)
                            if (Snakes[i].MaxLenght > snakeTime.MaxLenght)
                                snakeTime = Snakes[i];
                        snakesLenght[j] = snakeTime;
                        for (int i = 0; i < Snakes.Count; i++)
                            if (snakeTime == Snakes[i])
                            {
                                Snakes.RemoveAt(i);
                                break;
                            }
                    }
                    Snakes.Clear();

                    StopPoint = 0;
                    for (int i = 0; i < 100; i++)
                        Snakes.Add(new Snake(snakesLenght[i % 10].WallView, snakesLenght[i % 10].KindOfFood, rand));
                    for (int i = 80; i < 100; i++)
                    {
                        if (i < 90) Snakes[i].Mutation(6, rand);
                        else Snakes[i].Mutation(30, rand);
                    }
                    PlayingField = new Field[NextField.X, NextField.Y];
                    NewField();
                    Generation++;
                    SnakeManage = 0;
                    if (Generation % 100 == 0)
                        save();
                }
            }
        }
        public void DrawField()
        {
            if (pictureBox1.Height > 0 && pictureBox1.Width > 0 && ShowCheckBox.Checked)
            {
                if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
                Bitmap bmp = new Bitmap(PlayingField.GetLength(0) * 10, PlayingField.GetLength(1) * 10);
                Graphics g = Graphics.FromImage(bmp);
                for (int i = 0; i < PlayingField.GetLength(0); i++)
                    for (int j = 0; j < PlayingField.GetLength(1); j++)
                    {
                        if (PlayingField[i, j] == Field.Void) g.FillRectangle(new SolidBrush(Color.LightGray), i * 10, j * 10, 9, 9);
                        else if (PlayingField[i, j] == Field.Snake) g.FillRectangle(new SolidBrush(Color.Black), i * 10, j * 10, 9, 9);
                        else g.FillRectangle(new SolidBrush(Color.DarkRed), i * 10, j * 10, 9, 9);
                    }
                for (int i = 0; i < Snakes.Count; i++)
                    if (Snakes[i].IsAlive)
                        for (int j = 0; j < Snakes[i].TheBodyOfTheSnake.Count; j++)
                        {
                            Color[] colors = { Color.Green, Color.DarkGreen, Color.Blue, Color.DarkBlue };
                            int p = 0;
                            if (Snakes[i].isMutat) p = 2;
                            if (j != Snakes[i].TheBodyOfTheSnake.Count - 1)
                                g.FillRectangle(new SolidBrush(colors[p]), Snakes[i].TheBodyOfTheSnake[j].X * 10,
                                    Snakes[i].TheBodyOfTheSnake[j].Y * 10, 9, 9);
                            else
                                g.FillRectangle(new SolidBrush(colors[p + 1]), Snakes[i].TheBodyOfTheSnake[j].X * 10,
                                    Snakes[i].TheBodyOfTheSnake[j].Y * 10, 9, 9);
                        }
                pictureBox1.Image = bmp;
            }
        }
        public void NewField()
        {
            for (int i = 0; i < PlayingField.GetLength(0); i++)
                for (int j = 0; j < PlayingField.GetLength(1); j++)
                {
                    if (i == 0 || j == 0 || i == PlayingField.GetLength(0) - 1 ||
                        j == PlayingField.GetLength(1) - 1)
                        PlayingField[i, j] = Field.Snake;
                    else
                        PlayingField[i, j] = Field.Void;
                }
            for (int i = 0; i < Snakes.Count; i++)
            {
                while (true)
                {
                    int X = rand.Next(1, PlayingField.GetLength(0) - 1);
                    int Y = rand.Next(1, PlayingField.GetLength(1) - 1);
                    int x = 0, y = 0;
                    if (Snakes[i].Angle == Angle.Up) y++;
                    else if (Snakes[i].Angle == Angle.Down) y--;
                    else if (Snakes[i].Angle == Angle.Right) x--;
                    else x++;
                    try
                    {
                        if (PlayingField[X, Y] == Field.Void && PlayingField[X + x, Y + y] == Field.Void &&
                            PlayingField[X + x + x, Y + y + y] == Field.Void)
                        {
                            PlayingField[X, Y] = Field.Snake;
                            PlayingField[X + x, Y + y] = Field.Snake;
                            PlayingField[X + x + x, Y + y + y] = Field.Snake;
                            Snakes[i].TheBodyOfTheSnake.Add(new Point(X + x + x, Y + y + y));
                            Snakes[i].TheBodyOfTheSnake.Add(new Point(X + x, Y + y));
                            Snakes[i].TheBodyOfTheSnake.Add(new Point(X, Y));
                            break;
                        }
                    }
                    catch { }
                }
            }
            for (int i = 0; i < 100; i++)
            {
                while (true)
                {
                    int X = rand.Next(1, PlayingField.GetLength(0) - 1);
                    int Y = rand.Next(1, PlayingField.GetLength(1) - 1);
                    if (PlayingField[X, Y] == Field.Void)
                    {
                        PlayingField[X, Y] = Field.Eat;
                        break;
                    }
                }
            }
            DrawField();
        }
        public void save()
        {
            if (FileName != null && AutoSave)
            {
                string save = "";

                save += Generation + " ";
                save += StopPoint + " ";
                save += PlayingField.GetLength(0) + " ";
                save += PlayingField.GetLength(1) + "q";

                for (int j = 1; j < PlayingField.GetLength(1) - 1; j++)
                    for (int i = 1; i < PlayingField.GetLength(0) - 1; i++)
                    {
                        if (PlayingField[i, j] == Field.Void) save += "0 ";
                        else if (PlayingField[i, j] == Field.Snake) save += "1 ";
                        else if (PlayingField[i, j] == Field.Eat) save += "2 ";
                    }
                save += "q";

                for (int i = 0; i < Snakes.Count; i++)
                {
                    if (i != 0) save += "y";
                    for (int i1 = 0; i1 < 6; i1++)
                        for (int i2 = 0; i2 < 11; i2++)
                            for (int i3 = 0; i3 < 3; i3++)
                            {
                                save += Snakes[i].WallView[i1, i2, i3] + " ";
                                save += Snakes[i].KindOfFood[i1, i2, i3] + " ";
                            }
                    save += Snakes[i].Energy + " ";
                    save += Snakes[i].Angle + " ";
                    save += Snakes[i].IsAlive + " ";
                    for (int j = 0; j < Snakes[i].TheBodyOfTheSnake.Count; j++)
                    {
                        save += Snakes[i].TheBodyOfTheSnake[j].X + " ";
                        save += Snakes[i].TheBodyOfTheSnake[j].Y;
                        if (j < Snakes[i].TheBodyOfTheSnake.Count - 1) save += " ";
                    }
                }

                System.IO.File.WriteAllText(FileName, save);
            }
        }

        public FormMain()
        {
            InitializeComponent();
            CounterSnakeLabel.Text = Convert.ToString(StopPoint);
            t1 = new Thread(Simulation);
            for (int i = 0; i < 100; i++)
                Snakes.Add(new Snake(rand));
            NewField();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            GenerationLabel.Text = Convert.ToString(Generation);
            int k = 0;
            for (int i = 0; i < Snakes.Count; i++)
                if (Snakes[i].IsAlive) k++;
            AllSnakesLabel.Text = Convert.ToString(k);
        }
        private void ResetButton_Click(object sender, EventArgs e)
        {
            Stop = true;
            t1.Abort();
            StartStopButton.Text = "Start";
            Snakes.Clear();
            Generation = 0;
            StopPoint = 0;
            for (int i = 0; i < 100; i++)
                Snakes.Add(new Snake(rand));
            PlayingField = new Field[NextField.X, NextField.Y];
            NewField();
            GenerationLabel.Text = Convert.ToString(Generation);
            int k = 0;
            for (int i = 0; i < Snakes.Count; i++)
                if (Snakes[i].IsAlive) k++;
            AllSnakesLabel.Text = Convert.ToString(k);
            CounterSnakeLabel.Text = Convert.ToString(StopPoint);
            DrawField();
        }
        private void OptionsButton_Click(object sender, EventArgs e)
        {
            if (!Stop)
            {
                Stop = true;
                StartStopButton.Text = "Start";
                Thread.Sleep(80);
            }
            FormOptions form = new FormOptions(this);
            form.ShowDialog();
            DrawField();
        }
        private void StartStopButton_Click(object sender, EventArgs e)
        {
            if (Stop)
            {
                Stop = false;
                StartStopButton.Text = "Stop";
                try
                {
                    t1.Start();
                }
                catch
                {
                    t1.Abort();
                    t1 = new Thread(Simulation);
                    t1.Start();
                }
            }
            else
            {
                Stop = true;
                StartStopButton.Text = "Start";
                Thread.Sleep(80);
            }
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t1 != null) t1.Abort();
        }
    }

    public class Snake
    {
        public List<Point> TheBodyOfTheSnake = new List<Point>();
        public Angle Angle = Angle.Up;
        public bool IsAlive = true;
        public bool isMutat = false;
        public int Energy = 20;
        public int MaxLenght = 3;
        public int[,,] WallView = new int[6, 11, 3];
        public int[,,] KindOfFood = new int[6, 11, 3];
        public Snake(Random rand)
        {
            for (int i1 = 0; i1 < 6; i1++)
                for (int i2 = 0; i2 < 11; i2++)
                    for (int i3 = 0; i3 < 3; i3++)
                    {
                        WallView[i1, i2, i3] = rand.Next(-2, 3);
                        KindOfFood[i1, i2, i3] = rand.Next(-2, 3);
                    }
            int t = rand.Next(4);
            if (t == 0) Angle = Angle.Up;
            else if (t == 1) Angle = Angle.Right;
            else if (t == 2) Angle = Angle.Down;
            else Angle = Angle.Left;
        }
        public Snake(int[,,] WallView, int[,,] KindOfFood, Random rand)
        {
            this.WallView = WallView;
            this.KindOfFood = KindOfFood;
            int t = rand.Next(4);
            if (t == 0) Angle = Angle.Up;
            else if (t == 1) Angle = Angle.Right;
            else if (t == 2) Angle = Angle.Down;
            else Angle = Angle.Left;
        }
        public void Mutation(int NumberOf, Random rand)
        {
            for (int i = 0; i < NumberOf; i++)
            {
                int i1, i2, i3;
                while (true)
                {
                    i1 = rand.Next(6);
                    i2 = rand.Next(11);
                    i3 = rand.Next(3);
                    if (i1 != 5 || i2 != 5) break;
                }
                WallView[i1, i2, i3] = rand.Next(-100, 101);
                while (true)
                {
                    i1 = rand.Next(6);
                    i2 = rand.Next(11);
                    i3 = rand.Next(3);
                    if (i1 != 5 || i2 != 5) break;
                }
                KindOfFood[i1, i2, i3] = rand.Next(-100, 101) * 2;
            }
            isMutat = true;
        }
        public Angle WhereToGo(Field[,] field, Random rand)
        {
            int Up = 0, Right = 0, Left = 0;
            for (int y = 0; y < 11; y++)
                for (int x = 0; x < 11; x++)
                    if (x != 5 || y != 5)
                        if (field[x, y] == Field.Eat)
                            if (x < 6)
                            {
                                Left += KindOfFood[x, y, 0];
                                Up += KindOfFood[x, y, 1];
                                Right += KindOfFood[x, y, 2];
                            }
                            else
                            {
                                Left += KindOfFood[10 - x, y, 2];
                                Up += KindOfFood[10 - x, y, 1];
                                Right += KindOfFood[10 - x, y, 0];
                            }
                        else if (field[x, y] == Field.Snake)
                            if (x < 6)
                            {
                                Left += WallView[x, y, 0];
                                Up += WallView[x, y, 1];
                                Right += WallView[x, y, 2];
                            }
                            else
                            {
                                Left += WallView[10 - x, y, 2];
                                Up += WallView[10 - x, y, 1];
                                Right += WallView[10 - x, y, 0];
                            }
            Angle newAngle = Angle.Up;
            if (Up > Right && Up > Left) newAngle = Angle.Up;
            else if (Right > Up && Right > Left) newAngle = Angle.Right;
            else if (Left > Up && Left > Right) newAngle = Angle.Left;
            else if (Up == Right && Right == Left)
            {
                int t = rand.Next(3);
                if (t == 0) newAngle = Angle.Left;
                else if (t == 1) newAngle = Angle.Up;
                else newAngle = Angle.Right;
            }
            else if (Up < Right && Up < Left)
            {
                int t = rand.Next(2);
                if (t == 0) newAngle = Angle.Left;
                else newAngle = Angle.Right;
            }
            else if (Right < Up && Right < Left)
            {
                int t = rand.Next(2);
                if (t == 0) newAngle = Angle.Left;
                else newAngle = Angle.Up;
            }
            else if (Left < Up && Left < Right)
            {
                int t = rand.Next(2);
                if (t == 0) newAngle = Angle.Up;
                else newAngle = Angle.Right;
            }
            return newAngle;
        }
        public Angle ChangeAngle(Angle newAngle)
        {
            if ((Angle == Angle.Up && newAngle == Angle.Up) || (Angle == Angle.Left && newAngle == Angle.Right) ||
                (Angle == Angle.Right && newAngle == Angle.Left)) Angle = Angle.Up;
            else if ((Angle == Angle.Down && newAngle == Angle.Up) || (Angle == Angle.Right && newAngle == Angle.Right) ||
                (Angle == Angle.Left && newAngle == Angle.Left)) Angle = Angle.Down;
            else if ((Angle == Angle.Right && newAngle == Angle.Up) || (Angle == Angle.Up && newAngle == Angle.Right) ||
                (Angle == Angle.Down && newAngle == Angle.Left)) Angle = Angle.Right;
            else Angle = Angle.Left;
            return Angle;
        }
        public void Go(Field newCell)
        {
            int x = 0, y = 0;
            if (Angle == Angle.Up) y--;
            else if (Angle == Angle.Down) y++;
            else if (Angle == Angle.Right) x++;
            else x--;
            if (newCell != Field.Snake)
                TheBodyOfTheSnake.Add(new Point(TheBodyOfTheSnake[TheBodyOfTheSnake.Count - 1].X + x,
                    TheBodyOfTheSnake[TheBodyOfTheSnake.Count - 1].Y + y));
            if (newCell == Field.Void)
                TheBodyOfTheSnake.RemoveAt(0);
            if (newCell == Field.Eat) Energy += 20;
            if (newCell == Field.Snake)
                IsAlive = false;
            else
            {
                Energy--;
                if (Energy == 0 && TheBodyOfTheSnake.Count > 2)
                {
                    Energy = 20;
                    TheBodyOfTheSnake.RemoveAt(0);
                }
                else if (Energy == 0)
                    IsAlive = false;
            }
            MaxLenght = Math.Max(MaxLenght, TheBodyOfTheSnake.Count);
        }
        public static Field[,] GetView(Field[,] field, Angle Angle)
        {
            Field[,] newField = new Field[11, 11];
            for (int y = 0; y < 11; y++)
                for (int x = 0; x < 11; x++)
                {
                    if (Angle == Angle.Up)
                        newField[x, y] = field[x, y];
                    else if (Angle == Angle.Down)
                        newField[x, y] = field[10 - x, 10 - y];
                    else if (Angle == Angle.Right)
                        newField[x, y] = field[10 - y, x];
                    else if (Angle == Angle.Left)
                        newField[x, y] = field[y, 10 - x];
                }
            return newField;
        }
        public Angle MakeYourMove(Angle angle, Field[,] field, Random rand)
        {
            Angle SnakeAngle = WhereToGo(Snake.GetView(field, Angle), rand);
            if (SnakeAngle != angle)
            {
                for (int y = 0; y < 11; y++)
                    for (int x = 0; x < 11; x++)
                        if (x != 5 || y != 5)
                            if (GetView(field, Angle)[x, y] == Field.Eat)
                                if (x < 6)
                                {
                                    if (SnakeAngle == Angle.Left)
                                        KindOfFood[x, y, 0] -= 15;
                                    if (SnakeAngle == Angle.Up)
                                        KindOfFood[x, y, 1] -= 15;
                                    if (SnakeAngle == Angle.Right)
                                        KindOfFood[x, y, 2] -= 15;

                                    if (angle == Angle.Left)
                                        KindOfFood[x, y, 0] += 15;
                                    if (angle == Angle.Up)
                                        KindOfFood[x, y, 1] += 15;
                                    if (angle == Angle.Right)
                                        KindOfFood[x, y, 2] += 15;
                                }
                                else
                                {
                                    if (SnakeAngle == Angle.Left)
                                        KindOfFood[10 - x, y, 2] -= 15;
                                    if (SnakeAngle == Angle.Up)
                                        KindOfFood[10 - x, y, 1] -= 15;
                                    if (SnakeAngle == Angle.Right)
                                        KindOfFood[10 - x, y, 0] -= 15;

                                    if (angle == Angle.Left)
                                        KindOfFood[10 - x, y, 2] += 15;
                                    if (angle == Angle.Up)
                                        KindOfFood[10 - x, y, 1] += 15;
                                    if (angle == Angle.Right)
                                        KindOfFood[10 - x, y, 0] += 15;
                                }
                            else if (GetView(field, Angle)[x, y] == Field.Snake)
                                if (x < 6)
                                {
                                    if (SnakeAngle == Angle.Left)
                                        WallView[x, y, 0] -= 5;
                                    if (SnakeAngle == Angle.Up)
                                        WallView[x, y, 1] -= 5;
                                    if (SnakeAngle == Angle.Right)
                                        WallView[x, y, 2] -= 5;

                                    if (angle == Angle.Left)
                                        WallView[x, y, 0] += 5;
                                    if (angle == Angle.Up)
                                        WallView[x, y, 1] += 5;
                                    if (angle == Angle.Right)
                                        WallView[x, y, 2] += 5;
                                }
                                else
                                {
                                    if (SnakeAngle == Angle.Left)
                                        WallView[10 - x, y, 2] -= 5;
                                    if (SnakeAngle == Angle.Up)
                                        WallView[10 - x, y, 1] -= 5;
                                    if (SnakeAngle == Angle.Right)
                                        WallView[10 - x, y, 0] -= 5;

                                    if (angle == Angle.Left)
                                        WallView[10 - x, y, 2] += 5;
                                    if (angle == Angle.Up)
                                        WallView[10 - x, y, 1] += 5;
                                    if (angle == Angle.Right)
                                        WallView[10 - x, y, 0] += 5;
                                }
            }
            return ChangeAngle(angle);
        }
    }

    public enum Angle
    {
        Up, Down, Right, Left
    }
    public enum Field
    {
        Void, Eat, Snake
    }
}
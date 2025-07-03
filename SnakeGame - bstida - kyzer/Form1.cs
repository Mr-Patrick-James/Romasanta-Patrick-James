using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        private int maxWidth;
        private int maxHeight;
        private int score = 0;
        private string direction = "right";
        private Random rand = new Random();
        private int highScore = 0;



        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.ClientSize = new Size(800, 640); // 40 tiles * 20px + HUD
            this.BackColor = Color.Black;
            this.KeyPreview = true;

            StartGame();
           
        }

        private void StartGame()
        {
            Snake.Clear();
            direction = "right";
            score = 0;

            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head);

            food = new Circle { X = rand.Next(0, 40), Y = rand.Next(0, 30) };

            gameTimer.Start();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
            Invalidate(); // Triggers Paint event
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw top HUD
            g.FillRectangle(new SolidBrush(Color.FromArgb(113, 140, 0)), 0, 0, 800, 40);
            Font font = new Font("Segoe UI", 12, FontStyle.Bold);
            g.DrawString("🍎 " + score, font, Brushes.White, 10, 10);
            g.DrawString("🏆 " + highScore, font, Brushes.White, 120, 10);
            g.DrawString("🔊", font, Brushes.White, 750, 10); // Decorative

            // Draw checkered grid
            for (int y = 0; y < 30; y++)
            {
                for (int x = 0; x < 40; x++)
                {
                    bool light = (x + y) % 2 == 0;
                    Brush b = light ? new SolidBrush(Color.FromArgb(170, 215, 81)) : new SolidBrush(Color.FromArgb(162, 209, 73));
                    g.FillRectangle(b, x * 20, y * 20 + 40, 20, 20);
                }
            }

            // Draw snake
            for (int i = 0; i < Snake.Count; i++)
            {
                Brush brush = i == 0 ? Brushes.Blue : Brushes.DeepSkyBlue;
                Rectangle rect = new Rectangle(Snake[i].X * 20, Snake[i].Y * 20 + 40, 20, 20);
                g.FillEllipse(brush, rect);

                // Draw eyes on head
                if (i == 0)
                {
                    g.FillEllipse(Brushes.White, rect.Left + 4, rect.Top + 4, 4, 4);
                    g.FillEllipse(Brushes.White, rect.Right - 8, rect.Top + 4, 4, 4);
                    g.FillEllipse(Brushes.Black, rect.Left + 5, rect.Top + 5, 2, 2);
                    g.FillEllipse(Brushes.Black, rect.Right - 7, rect.Top + 5, 2, 2);
                }
            }

            // Draw apple with leaf
            int fx = food.X * 20;
            int fy = food.Y * 20 + 40;
            g.FillEllipse(Brushes.Red, fx, fy, 20, 20);
            Point[] leaf = {
        new Point(fx + 10, fy),
        new Point(fx + 13, fy - 6),
        new Point(fx + 7, fy - 6)
    };
            g.FillPolygon(Brushes.ForestGreen, leaf);
        }

        private void MoveSnake()
        {
            for (int i = Snake.Count - 1; i > 0; i--)
            {
                Snake[i].X = Snake[i - 1].X;
                Snake[i].Y = Snake[i - 1].Y;
            }
            if (Snake[0].X == food.X && Snake[0].Y == food.Y)
            {
                Snake.Add(new Circle { X = Snake[Snake.Count - 1].X, Y = Snake[Snake.Count - 1].Y });
                food = new Circle { X = rand.Next(0, 40), Y = rand.Next(0, 30) };
                score++;
                if (score > highScore) highScore = score;
            }


            switch (direction)
            {
                case "up": Snake[0].Y--; break;
                case "down": Snake[0].Y++; break;
                case "left": Snake[0].X--; break;
                case "right": Snake[0].X++; break;
            }

            // Check collision with wall
            if (Snake[0].X < 0 || Snake[0].Y < 0 || Snake[0].X >= 40 || Snake[0].Y >= 30)
                GameOver();

            // Check collision with self
            for (int i = 1; i < Snake.Count; i++)
                if (Snake[0].X == Snake[i].X && Snake[0].Y == Snake[i].Y)
                    GameOver();

            // Check food collision
            if (Snake[0].X == food.X && Snake[0].Y == food.Y)
            {
                Snake.Add(new Circle { X = Snake[Snake.Count - 1].X, Y = Snake[Snake.Count - 1].Y });
                food = new Circle { X = rand.Next(0, 40), Y = rand.Next(0, 30) };
                score++;
                Text = $"Snake Game - Score: {score}";
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != "down") direction = "up";
                    break;
                case Keys.Down:
                    if (direction != "up") direction = "down";
                    break;
                case Keys.Left:
                    if (direction != "right") direction = "left";
                    break;
                case Keys.Right:
                    if (direction != "left") direction = "right";
                    break;
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show($"Game Over! Your score: {score}");
            StartGame();
        }

        private class Circle
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}

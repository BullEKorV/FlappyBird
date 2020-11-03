using System;
using Raylib_cs;
using System.Collections.Generic;

namespace Graphics
{
    public class Program
    {
        static Random random = new Random();
        static int windowHeight = 800;
        static int windowLength = 1000;
        static Rectangle p1 = new Rectangle(windowLength / 4, windowHeight / 2, 50, 50);
        static List<Obstacle> obstacles = new List<Obstacle>();
        static float x;
        static float yVelocity;
        static int score = 0;
        static int highScore;
        static int difficulty = 0;
        static string gameState = "Menu";
        static void Main(string[] args)
        {
            Raylib.SetTargetFPS(120);
            x = 0;
            Raylib.InitWindow(windowLength, windowHeight, "Flappy bird");
            int timer = 0;

            while (!Raylib.WindowShouldClose())
            {
                if (gameState == "Menu")
                {
                    try
                    {
                        highScore = int.Parse(System.IO.File.ReadAllText(@"highScore.txt"));
                    }
                    catch (System.Exception)
                    {
                        highScore = 0;
                    }
                    difficulty = 0;
                    score = 0;
                    timer = 0;
                    obstacles.Clear();
                    x = 0;
                    p1.y = windowHeight / 2;
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                    {
                        gameState = "Game";
                        SpawnObstacles();
                    }
                }
                if (gameState == "Game")
                {
                    x -= 2;
                    DespawnObstacles();
                    difficulty = score / 10;
                    if (difficulty == 11) difficulty = 10;
                    if (obstacles.Count < 20)
                    {
                        timer++;
                        if (timer >= 360 - difficulty * 20)
                        {
                            SpawnObstacles();
                            timer = 0;
                        }
                    }
                    CheckKeyPresses();
                    UpdatePos();
                    CheckCollision();
                }
                if (gameState == "GameOver")
                {
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                    {
                        gameState = "Menu";
                    }
                }
                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.WHITE);
                Raylib.DrawRectangle((int)p1.x, (int)p1.y, (int)p1.width, (int)p1.height, Color.RED);
                for (int i = 0; i < obstacles.Count; i++)
                {
                    Obstacle obstacle = obstacles[i];
                    Raylib.DrawRectangle((int)obstacle.x + (int)x, (int)obstacle.y, (int)obstacle.width, (int)obstacle.height, Color.GREEN);
                }
                if (gameState == "GameOver") Raylib.DrawText("You died", windowLength / 2 - 110, windowHeight / 2, 80, Color.RED);
                Raylib.DrawText("Score: " + score, 10, 10, 30, Color.ORANGE);
                Raylib.DrawText("Highscore: " + highScore, windowLength - 230, 10, 30, Color.ORANGE);
                if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) Raylib.DrawText("Bruh", 15, windowHeight / 2, 400, Color.BLACK);
                Raylib.EndDrawing();
            }
        }
        static void DespawnObstacles()
        {
            for (int i = 0; i < obstacles.Count; i++)
            {
                Obstacle obstacle = obstacles[i];
                if (obstacle.x < -x - obstacle.width)
                {
                    obstacles.RemoveAt(i);
                }
                if (obstacle.x == -x + windowLength / 4 - 150 && obstacle.y == 0)
                {
                    score++;
                }
            }
        }
        static void SpawnObstacles()
        {
            int height = random.Next(50, windowHeight - 400 - difficulty * 10);
            int distance = random.Next(280 - difficulty * 10, 380 - difficulty * 18);
            int width = random.Next(120, 200);
            obstacles.Add(new Obstacle(-x + windowLength, 0, width, height));
            obstacles.Add(new Obstacle(-x + windowLength, height + distance, width, windowHeight));
        }
        static void CheckKeyPresses()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) Jump();
        }
        static void Jump()
        {
            yVelocity = 8;
        }
        static void UpdatePos()
        {
            yVelocity -= (float)0.25;
            p1.y -= yVelocity;
        }
        static void CheckCollision()
        {
            for (int i = 0; i < obstacles.Count; i++)
            {
                Obstacle obstacle = obstacles[i];
                Rectangle r2 = new Rectangle(obstacle.x + (int)x, obstacle.y, obstacle.width, obstacle.height);
                bool isOverlapping = Raylib.CheckCollisionRecs(p1, r2);
                if (isOverlapping || p1.y + p1.height > windowHeight || p1.y < 0)
                {
                    if (score > highScore)
                    {
                        highScore = score;
                        System.IO.File.WriteAllText(@"highScore.txt", highScore.ToString());
                    }
                    gameState = "GameOver";
                }
            }
        }
    }
    public class Obstacle
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public Obstacle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Snake
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(32, 16, 5, 500);
            game.Run();
        }
    }

    class Game
    {
        private int screenWidth;
        private int screenHeight;
        private int score;
        private int frameRate;
        private bool isGameOver;
        private Pixel snakeHead;
        private List<Pixel> snakeBody;
        private Pixel berry;
        private Direction direction;
        private DateTime lastFrameTime;
        private Random random;

        public Game(int screenWidth, int screenHeight, int initialScore, int frameRate)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            score = initialScore;
            this.frameRate = frameRate;
            InitializeGame();
        }

        private void InitializeGame()
        {
            Console.SetWindowSize(screenWidth, screenHeight);
            isGameOver = false;
            snakeHead = new Pixel(screenWidth / 2, screenHeight / 2, ConsoleColor.Red);
            snakeBody = new List<Pixel>();
            random = new Random();
            berry = GenerateBerry();
            direction = Direction.RIGHT;
        }

        public void Run()
        {
            while(!isGameOver)
            {
                HandleInput();
                Update();
            }

            showGameOverMessage();
        }

        private void PaintScene()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.White;
            for(int i = 0; i < screenWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, screenHeight - 1);
                Console.Write("■");
            }
            for(int i = 0; i < screenHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(screenWidth - 1, i);
                Console.Write("■");
            }

            DrawPixel(berry);
            DrawPixel(snakeHead);

            foreach(var bodyPixel in snakeBody)
            {
                DrawPixel(bodyPixel);
            }

            Console.SetCursorPosition(0, 0);
        }

        private void DrawPixel(Pixel pixel)
        {
            Console.SetCursorPosition(pixel.xPos, pixel.yPos);
            Console.ForegroundColor = pixel.color;
            Console.Write("■");
        }

        private void showGameOverMessage()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(6, 3);
            Console.WriteLine("GAME OVER, score: {0}", score);
            Console.SetCursorPosition(0, 0);
            Console.ReadKey();
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.RightArrow when direction != Direction.LEFT:
                        direction = Direction.RIGHT;
                        break;
                    case ConsoleKey.DownArrow when direction != Direction.UP:
                        direction = Direction.DOWN;
                        break;
                    case ConsoleKey.LeftArrow when direction != Direction.RIGHT:
                        direction = Direction.LEFT;
                        break;
                    case ConsoleKey.UpArrow when direction != Direction.DOWN:
                        direction = Direction.UP;
                        break;
                }
            }
        }

        private void Update()
        {
            if ((DateTime.Now - lastFrameTime).TotalMilliseconds < frameRate)
            {
                return;
            }

            lastFrameTime = DateTime.Now;

            MoveSnake();
            CheckCollision();
            CheckBerryCollision();

            PaintScene();
        }

        private void MoveSnake()
        {  
            snakeBody.Add(new Pixel(snakeHead.xPos, snakeHead.yPos, ConsoleColor.Green));

            switch (direction)
            {
                case Direction.RIGHT:
                    snakeHead.xPos++;
                    break;
                case Direction.DOWN:
                    snakeHead.yPos++;
                    break;
                case Direction.LEFT:
                    snakeHead.xPos--;
                    break;
                case Direction.UP:
                    snakeHead.yPos--;
                    break;
            }

            if (snakeBody.Count() > score)
            {
                snakeBody.RemoveAt(0);
            }
        }

        private void CheckCollision()
        {
            if(snakeHead.xPos == 0 || snakeHead.xPos == screenWidth - 1 ||
                snakeHead.yPos==0 || snakeHead.yPos == screenHeight - 1)
            {
                isGameOver = true;
                return;
            }

            foreach(var bodyPixel in snakeBody)
            {
                if(snakeHead.xPos == bodyPixel.xPos && snakeHead.yPos == bodyPixel.yPos)
                {
                    isGameOver = true;
                    return;
                }
            }
        }

        private void CheckBerryCollision()
        {
            if(snakeHead.xPos == berry.xPos && snakeHead.yPos == berry.yPos)
            {
                score++;
                berry = GenerateBerry();
            }
        }

        private Pixel GenerateBerry()
        {
            return new Pixel(random.Next(1, screenWidth - 1), random.Next(1, screenHeight - 1), ConsoleColor.Cyan);
        }
    }

    class Pixel
    {
        public int xPos { get; set; }
        public int yPos { get; set; }
        public ConsoleColor color { get; set; }

        public Pixel(int xPos, int yPos, ConsoleColor color)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.color = color;
        }
    }

    enum Direction
    {
        RIGHT,
        DOWN,
        LEFT,
        UP
    }
}

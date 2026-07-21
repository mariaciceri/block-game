namespace PuzzleGame;

public class GamePlay
{
    public void GenerateGrid(int width, int height)
    {
        int[,] grid = new int[width, height];

        Random rnd = new Random();
        int randWidth = rnd.Next(1, width - 1);
        int randHeight = rnd.Next(1, height - 1);
        
        int randLocationX = rnd.Next(width - 1);
        int randLocationY = rnd.Next(height - 1);
        
        Console.WriteLine($"{randLocationX}x{randLocationY} - {randWidth}x{randHeight}");
        
        BlockInsertion(randLocationX, randLocationY, grid, randHeight, 1);
   
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Console.Write(grid[x,y] + " ");
            }
            Console.WriteLine();
        }
    }

    public void BlockInsertion(int x, int y, int[,] grid, int blockHeight, int blockWidth)
    {
        if (blockWidth == 1)
        {
            for (int h = 1; h <= blockHeight; h++)
            {
                if (grid[x, y] == 0)
                {
                    grid[x, y] = 1;
                    y++;
                }
                else
                {
                    break; //must clean up what came before
                }
            }
        }
        else if (blockHeight == 1)
        {
            for (int w = 1; w <= blockWidth; w++)
            {
                if (grid[x, y] == 0)
                {
                    grid[x, y] = 1;
                    x++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
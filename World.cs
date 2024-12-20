namespace GUI.Client.Models;

/// <summary>
///  This class stores a representation of the world; it includes the player's ID,
///  the world's size as wells as collections of walls, snakes, and powerups.
/// </summary>
/// <author>
///  Benjamin Keefer and Madi Murray
/// </author>
/// <version>
///  November 22nd, 2024
/// </version>
public class World
{
    /// <summary> Stores all of the walls in the world </summary>
    public List<Wall> Walls;
    /// <summary> Stores all of the snakes in the world </summary>
    public Dictionary<int, Snake> snakes;
    /// <summary> Stores all of the powerups in the world </summary>
    public Dictionary<int, Powerup> powerups;
    
    /// <summary>
    ///  Holds the ID of the player's snake that the canvas is centered around
    /// </summary>
    public int thisPlayerID { get; set; }
    /// <summary> Stores the size (width and height) of the world provided by the server </summary>
    public int worldSize{get; set;}

    /// <summary> Zero parameter constructor for the world </summary>
    public World()
    {
        Walls = new();
        snakes = new();
        powerups = new();
    }

    /// <summary>
    ///  Constructs a world from a pre-existing world
    /// </summary>
    /// <param name="world"></param>
    public World(World world)
    {
        snakes = new(world.snakes);
        powerups = new(world.powerups);
        Walls = new(world.Walls);
        worldSize = world.worldSize;
        thisPlayerID = world.thisPlayerID;
    }

    /// <summary>
    ///  Adds a new snake to the world, or updates it if it already exists
    /// </summary>
    /// <param name="snake"></param>
    public void AddSnake(Snake snake)
    {
        if(snakes.ContainsKey(snake.snake))
        {
            snakes[snake.snake] = snake;
        }
        else
        {
            snakes.Add(snake.snake, snake);
        }
    }

    /// <summary>
    ///  Returns the head of the snake that the canvas is centered around.
    /// </summary>
    /// <returns> Point2D containing the coordinates </returns>
    public Point2D? getThisSnakeHead()
    {
        if(!snakes.ContainsKey(thisPlayerID))
        {
            return null;
        }
        return snakes[thisPlayerID].body[snakes[thisPlayerID].body.Count - 1];
    }

    /// <summary>
    ///  Adds a new powerup to the world, or updates it if it already exists
    /// </summary>
    /// <param name="powerup"></param>
    public void AddPowerup(Powerup powerup)
    {
        if(powerups.ContainsKey(powerup.power))
        {
            powerups[powerup.power] = powerup;
            return;
        }
        powerups.Add(powerup.power, powerup);
    }

    /// <summary>
    ///  Adds a new wall to the world
    /// </summary>
    /// <param name="wall"></param>
    public void AddWall(Wall wall)
    {
        Walls.Add(wall);
    }
}

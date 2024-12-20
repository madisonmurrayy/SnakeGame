using GUI.Client.Models;
using Mysqlx.Crud;
using System.Text.Json;
using ZstdSharp.Unsafe;

namespace GUI.Client.Controllers;
/// <summary>
///   Parses information received from the network and updates the model of the
///   game world appropriately.
///   
///   Database password: C_sharper
/// </summary>
/// <author>
///  Benjamin Keefer and Madi Murray
/// </author>
/// <version>
///  December 5th, 2024
/// </version>
public class NetworkController
{
    /// <summary>
    ///  The connection between the player and the server.
    /// </summary>
    public NetworkConnection connection;
     
    /// <summary>
    ///  The world representation of the game
    /// </summary>
    public World? world;

    /// <summary>
    ///  The ID in the SQL database of the current game being played.
    /// </summary>
    public string currentGameID = "";

    /// <summary>
    ///  Constructor for a new network controller.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="address"></param>
    /// <param name="port"></param>
    public NetworkController(string name, string address, int port)
    {
        connection = new NetworkConnection();
        connection.Connect(address, port);
        connection.Name = name;
        connection.Send(connection.Name);
    }

    /// <summary>
    ///  Converts a key press (w,a,s,d) into a move instruction and sends
    ///  it to the server.
    /// </summary>
    /// <param name="direction"></param>
    public void Move(string direction)
    {
        string moving = "{\"moving\":";
        switch (direction)
        {
            case "w": connection.Send(moving + "\"up\"}"); return;
            case "a": connection.Send(moving + "\"left\"}"); return;
            case "s": connection.Send(moving + "\"down\"}"); return;
            case "d": connection.Send(moving + "\"right\"}"); return;
        }
    }

    /// <summary>
    ///  Begins the connection with the server by sending the player name
    /// </summary>
    public void StartConnection()
    {
        world = new();
            
        //reads the very first thing sent by the server: this snake/client's unique ID
        string ID = connection.ReadLine();
        //sets this player's snake id to the ID read from the server
        world.thisPlayerID = Int32.Parse(ID);

        //gets the world size
        string worldSize = connection.ReadLine();
        world.worldSize = Int32.Parse(worldSize);

        //read the rest of drawing info (snakes, walls, and powerups) from the server
        while (true)
        {
            string data;
            // Receives data from the server
            try
            {
                data = connection.ReadLine();
            }
            catch (Exception)
            {
                return;
            }
            // Lock keeps the world from updating while the canvas is being drawn
            lock (world)
            {
                // Case for deserializing a snake
                if (data.Contains("snake"))
                {
                    Snake? snake = JsonSerializer.Deserialize<Snake>(data);
                    if(snake!.score > snake.maxScore)
                    {
                        //update the maxScore to the database upon a new maxScore
                        snake.maxScore = snake.score;
                        connection.DBUpdateCommand("UPDATE Players SET maxScore = \"" + snake.maxScore +
                                "\" WHERE pID = " + snake.snake + " AND gID = " + currentGameID + ";");
                    }
                    if (snake != null)
                    {
                        //update each new snake into the database
                        world.AddSnake(snake);
                        if(snake.join == true)
                        {
                            connection.DBUpdateCommand($"INSERT INTO Players (gID, pID, playerName, maxScore, enterTime) values( \"{currentGameID}\"," +
                                $"\"{snake.snake}\", \"{snake.name}\", \"{snake.maxScore}\", \"{ DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") }\");");
                        }
                    }
                }
                // Case for deserializing a powerup
                else if (data.Contains("power"))
                {
                    Powerup? powerup = JsonSerializer.Deserialize<Powerup>(data);
                    if (powerup != null)
                    {
                        if (powerup.died == false)
                        {
                            world.AddPowerup(powerup);
                        }
                        else
                        {
                            world.powerups.Remove(powerup.power);
                        }
                    }
                }
                // Case for deserializing a wall
                else if (data.Contains("wall"))
                {
                    Wall? wall = JsonSerializer.Deserialize<Wall>(data);
                    if (wall != null)
                    {
                        world.AddWall(wall);
                    }
                }
            }
        }
    }
}
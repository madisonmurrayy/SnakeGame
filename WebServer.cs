using GUI.Client.Controllers;
using MySql.Data.MySqlClient;

namespace WebServer;
/// <summary>
///  This class contains the methods for receiving and sending html responses over the web.
///  Reads eel game data from the sql server to populate the html.
/// </summary>
/// <author> Benjamin Keefer and Madi Murray </author>
/// <version> December 5th, 2024 </version>
public static class WebServer
{
    /// <summary> The header for the beginning of the html response to the web user </summary>
    private static string okHeader = "HTTP/1.1 200 OK\r\nConnection: close\r\n"
        + "Content-Type: text/html; charset=UTF-8\r\nContent-Length: ";
    /// <summary> The header for the beginning of the page not found response </summary>
    private const string badHeader = "HTTP/1.1 404 Not Found\r\nConnection: close\r\n"
        + "Content-Type: text/html; charset=UTF-8\r\n\r\n 404: Error page not found";
    /// <summary> The information for connecting to the sql database </summary>
    private const string connectString = "server=atr.eng.utah.edu;database=u1434592;uid=u1434592;password=C_sharper";

    /// <summary>
    ///  Driver method for determining what response to send to the web user.
    /// </summary>
    public static void HandleHTTPRequest(NetworkConnection client)
    {
        // Reads the request from the web server.
        string request = "";
        try
        {
             request = client.ReadLine();
        }
        catch (Exception) {}
        
        // Handles case where user wants a specific game's details
        if (request.Contains("GET /games?gid="))
        {
            string[] words = request.Split("=");
            int gID = Int32.Parse(words[1].Split(" ")[0]);
            GameRequest(client, gID);
        }
        // Case for games page
        else if (request.Contains("GET /games"))
        {
            GamesRequest(client);
        }
        // Case for the home page
        else if (request.Contains("GET / "))
        {
            HomeRequest(client);
        }
        else
        {
            client.Send(badHeader);
        }
        client.Dispose();
    }

    /// <summary>
    ///  Constructs the html response containing the information from a single eel game.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="gID"></param>
    private static void GameRequest(NetworkConnection client, int gID)
    {
        string response =  "<html>\r\n  <h3>Stats for Game " + gID + " </h3>\r\n  <table border=\"1\">\r\n    <thead>\r\n" + 
            "      <tr>\r\n        <td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td>\r\n" +
            "      </tr>\r\n    </thead>\r\n    <tbody>";
        // Connects to the sql server.
        using (MySqlConnection conn = new MySqlConnection(connectString))
        {
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                // Sql query to get the right game
                command.CommandText = "SELECT * FROM Players WHERE gID = " + gID + ";";
                // Adds all of the game information to the html response.
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response += "<tr>";
                        response += "<td>" + reader["pID"] + "</td>";
                        response += "<td>" + reader["playerName"] + "</td>";
                        response += "<td>" + reader["maxScore"] + "</td>";
                        response += "<td>" + reader["enterTime"] + "</td>";
                        response += "<td>" + reader["leaveTime"] + "</td>";
                        response += "</tr>";
                    }
                }
            }
            catch (Exception) {}
        }

        response += "</tbody>\r\n  </table>\r\n</html>";
        string message = okHeader + response.Length + "\r\n\r\n" + response;
        // Sends message to the web user.
        client.Send(message);
    }

    /// <summary>
    ///  Constructs and sends the html response containing a table of all of the eel games.
    /// </summary>
    /// <param name="client"></param>
    private static void GamesRequest(NetworkConnection client)
    {
        // Response header
        string response = "<html>\r\n  <table border=\"1\">\r\n    <thead>\r\n      <tr>\r\n     " +
            "   <td>ID</td><td>Start</td><td>End</td>\r\n      </tr>\r\n    </thead>\r\n    " +
            "<tbody>\r\n ";

        // Connects to the sql server
        using (MySqlConnection conn = new MySqlConnection(connectString))
        {
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                // Query to get the table containing the games
                command.CommandText = "SELECT * FROM Games;";

                // Adds the game data to the html table
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response += "<tr>";
                        response += "<td><a href=\"/games?gid=" + reader["gID"] + "\">" + reader["gID"] + "</a></td>";
                        response += "<td>" + reader["startTime"] + "</td>";
                        response += "<td>" + reader["endTime"] + "</td>";
                        response += "</tr>";
                    }
                }
            }
            catch (Exception) {}
        }

        response +="</tbody>\r\n  </table>\r\n</html>";
        // Sends the final html response to the server
        string message = okHeader + response.Length + "\r\n\r\n" + response;
        client.Send(response);
    }

    /// <summary>
    ///  Sends the html for the home page to the web user.
    /// </summary>
    /// <param name="client"></param>
    private static void HomeRequest(NetworkConnection client)
    {
       string message = okHeader +"89 \r\n\r\n <html> <h3>Welcome to the Snake Games Database!</h3>" +
            "<a href=\"/games\">View Games</a></html>";
        client.Send(message);
    }
}

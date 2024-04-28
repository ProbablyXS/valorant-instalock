using System.Collections.Generic;
using System.Linq;
using valorant_instalock.Models;

namespace valorant_instalock.Classes
{
    public static class Agent
    {
        public static Coordinate SelectedAgent { get; set; }

        public static Dictionary<string, Coordinate> agentCoordinates = new Dictionary<string, Coordinate>
        {
        };

        public static Coordinate GetAgentCoordinatesByName(string agentName)
        => agentCoordinates.Where(c => c.Key == agentName).Select(c => c.Value).FirstOrDefault();

        public static string GetAgentNameByCoordinates(int X, int Y)
        => agentCoordinates.Where(c => c.Value.X == X && c.Value.Y == Y).Select(c => c.Key).FirstOrDefault();

        public static string[] getAgents()
        => agentCoordinates.Keys.ToArray();
    }
}
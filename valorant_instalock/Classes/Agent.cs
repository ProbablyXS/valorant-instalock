using valorant_instalock.Models;

namespace valorant_instalock.Classes
{
    public static class Agent
    {

        public static string[] AgentsList = {
            "Breach",
            "Neon",
            "Brimstone",
            "Cypher",
            "Jett",
            "Killjoy",
            "Omen",
            "Phoenix",
            "Raze",
            "Reyna",
            "Sage",
            "Sova",
            "Viper"
        };

        public static Coordinate SelectedAgent { get; set; }
        public static string SelectedagentName { get; set; }

        public static Coordinate GetAgentCoordinatesByName(string agentName)
        {
            //var newX = Convert.ToInt32(MyIni.Read(agentName).Split(',').First());
            //var newY = Convert.ToInt32(MyIni.Read(agentName).Split(',').Last());

            //return new Coordinate(newX, newY);
            return new Coordinate(0, 0);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using valorant_instalock.Models;

namespace valorant_instalock.Classes
{
    public static class Agent
    {
        static IniFile MyIni = new IniFile("agent.ini");


        public static Coordinate SelectedAgent { get; set; }
        public static string SelectedagentName { get; set; }

        public static Coordinate GetAgentCoordinatesByName(string agentName)
        {
            var newX = Convert.ToInt32(MyIni.Read(agentName).Split(',').First());
            var newY = Convert.ToInt32(MyIni.Read(agentName).Split(',').Last());

            return new Coordinate(newX, newY);
        }
    }
}
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private Int64 tickTimes = 0;
        private const int MarqueeTickDuration = 5;
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var args = Me.CustomData.Trim().Split('|');
            if (args.Length != 1)
            {
                Echo("args number error");
                return;
            }
            tickTimes++;
            if (tickTimes % MarqueeTickDuration == 0) {
                // every 5 ticks
                var marqueeArgs = args[0].Split('#');
                MarqueeController(marqueeArgs[0], int.Parse(marqueeArgs[1]), tickTimes);
            }
        }

        public void MarqueeController(string groupName, int oneSideCount, Int64 tickTimes)
        {
            var margueeGroup = GridTerminalSystem.GetBlockGroupWithName(groupName);
            List<IMyInteriorLight> margueeLights = new List<IMyInteriorLight>();
            margueeGroup.GetBlocksOfType(margueeLights);
            foreach(var light in margueeLights)
            {
                var round = ((tickTimes / MarqueeTickDuration) % oneSideCount) + 1;
                var lightIndex = int.Parse(light.CustomName.Trim().Split('-')[2].Trim());
                light.Enabled = lightIndex == round || lightIndex == round + oneSideCount;
            }
        }
    }
}

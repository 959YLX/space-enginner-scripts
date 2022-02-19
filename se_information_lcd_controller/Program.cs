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
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var args = Me.CustomData.Trim().Split('\n');
            if (args.Length != 2)
            {
                Echo("Script Custom Data MUST be 2 lines");
                return;
            }
            var groupNames = args[0].Split('#');
            List<String> displayLines = new List<string>();
            foreach(var groupName in groupNames)
            {
                var cargoContainerGroup = GridTerminalSystem.GetBlockGroupWithName(groupName);
                if (cargoContainerGroup == null)
                {
                    Echo($"cannot get group with name '{groupName}'");
                    continue;
                }
                List<IMyEntity> cargos = new List<IMyEntity>();
                cargoContainerGroup.GetBlocksOfType<IMyEntity>(cargos);
                MyFixedPoint total = new MyFixedPoint();
                MyFixedPoint current = new MyFixedPoint();
                foreach(var cargo in cargos)
                {
                    var inventory = cargo.GetInventory();
                    total += inventory.MaxVolume;
                    current += inventory.CurrentVolume;
                }
                var currentUsePercentage = Math.Round((((decimal)current) / ((decimal)total)) * 100, 2, MidpointRounding.AwayFromZero);
                displayLines.Add($"{groupName}: Max = {Math.Round(((decimal)total), 2, MidpointRounding.AwayFromZero)}");
                displayLines.Add($"Used = {currentUsePercentage}%, Remain = {Math.Round(100 - currentUsePercentage, 2, MidpointRounding.AwayFromZero)}%");
                displayLines.Add("");
            }
            IMyTextSurface display;
            var displayName = args[1];
            if (displayName.Contains("#"))
            {
                var displayArgs = displayName.Split('#');
                if (displayArgs.Length != 2)
                {
                    Echo($"cockpit name with index error: {displayName}");
                    return;
                }
                var cockpitName = displayArgs[0];
                var cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyTextSurfaceProvider;
                if (cockpit == null)
                {
                    Echo($"cannot get cockpit with name {cockpitName}");
                    return;
                }
                display = cockpit.GetSurface(int.Parse(displayArgs[1]));
            } else
            {
                display = GridTerminalSystem.GetBlockWithName(displayName) as IMyTextSurface;
            }
            SetTextToLCDDisplay(ref display, displayLines.ToArray());
        }
        public void SetTextToLCDDisplay(ref IMyTextSurface display, string[] lines)
        {
            if (display == null)
            {
                Echo("display is null");
                return;
            }
            StringBuilder builder = new StringBuilder();
            foreach (var line in lines)
            {
                builder.AppendLine(line);
            }
            if (display.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                display.ContentType = ContentType.TEXT_AND_IMAGE;
            }
            display.FontSize = 2.0F;
            display.WriteText(builder);
        }
    }
}

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

        private const char MethodDelimiter = ':';
        private const char MethodArgDelimiter = '#';
        private volatile uint tick = 0;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Trigger)
            {
                Echo($"run command by trigger: {argument}");
                var method = "";
                var methodArgs = new List<string>();
                GetArgs(argument, ref method, ref methodArgs);
                switch (method)
                {
                    case "LockAllDoor":
                        break;
                    default:
                        return;
                }
            }
            tick++;
            var args = Me.CustomData.Split('\n');
            foreach (var arg in args)
            {
                var method = "";
                var methodArgs = new List<string>();
                GetArgs(arg, ref method, ref methodArgs);
                switch(method)
                {
                    case "AutoDoor":
                        AutoDoor(methodArgs.ToArray());
                        break;
                    default:
                        Echo($"unknown method {method}");
                        continue;
                }
            }
        }

        public void AutoDoor(string[] args)
        {
            if (args.Length != 3) {
                Echo($"call AutoDoor Method args should be 3, now args = {PrintArray(args)}");
                return;
            }
            var isGrouop = bool.Parse(args[0]);
            var doorsName = args[1];
            var sensorsName = args[2];
            if (isGrouop) {
            } else
            {
                var door = GridTerminalSystem.GetBlockWithName(doorsName) as IMyDoor;
                var sensor = GridTerminalSystem.GetBlockWithName(sensorsName) as IMySensorBlock;
                if (door == null || sensor == null)
                {
                    Echo($"cannot get door or sensor by name {doorsName} and {sensorsName}");
                    return;
                }
                singleDoorController(ref door, ref sensor);
            }
        }

        private void singleDoorController(ref IMyDoor door, ref IMySensorBlock sensor)
        {
            if (sensor.LastDetectedEntity.IsEmpty())
            {
                if (door.Status == DoorStatus.Open || door.Status == DoorStatus.Opening)
                {
                    door.CloseDoor();
                }
            } else
            {
                if (door.Status == DoorStatus.Closed || door.Status == DoorStatus.Closing)
                {
                    door.OpenDoor();
                }
            }
        }

        public string PrintArray<T>(T[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            var length = args.Length;
            for (int i = 0; i < length; i++)
            {
                builder.Append(args[i].ToString());
                if (i != length - 1)
                {
                    builder.Append(',');
                }
            }
            builder.Append(']');
            return builder.ToString();
        }

        public void GetArgs(string arg, ref string method, ref List<string> args)
        {

            var methodWithName = arg.Split(MethodDelimiter);
            if (methodWithName.Length < 2)
            {
                Echo($"call 'AutoDoor' args error: {arg}");
                return;
            }
            method = methodWithName[0];
            args.Clear();
            var methodArgs = methodWithName[1].Split(MethodArgDelimiter);
            foreach (var methodArg in methodArgs)
            {
                args.Add(methodArg);
            }
        }
    }
}

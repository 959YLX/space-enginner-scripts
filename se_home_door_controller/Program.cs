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
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource != UpdateType.Update10)
            {
                return;
            }
            DetectAndControl("coolest");
            DetectAndControl("wfy");
        }

        public void DetectAndControl(string who)
        {
            if (who != "coolest" && who != "wfy")
            {
                Echo($"unsupport detect user {who}");
                return;
            }
            var internalSensor = GridTerminalSystem.GetBlockWithName($"{who}-home-internal-sensor") as IMySensorBlock;
            var outsideSensor = GridTerminalSystem.GetBlockWithName($"{who}-home-outside-sensor") as IMySensorBlock;
            var internalDetectedEntity = internalSensor.LastDetectedEntity;
            var outsideDetectedEntity = outsideSensor.LastDetectedEntity;
            if (internalDetectedEntity.Type != MyDetectedEntityType.CharacterHuman && outsideDetectedEntity.Type != MyDetectedEntityType.CharacterHuman)
            {
                SetDoorStatus(who, false, false);
                return;
            }
            if (internalDetectedEntity.Type == MyDetectedEntityType.CharacterHuman && outsideDetectedEntity.Type == MyDetectedEntityType.CharacterHuman)
            {
                Echo("unsupport auto open/close door when detected multi human");
                return;
            }
            if (internalDetectedEntity.Type == MyDetectedEntityType.CharacterHuman)
            {
                SetDoorStatus(who, true, false);
                return;
            }
            else
            {
                this.SetDoorStatus(who, false, true);
            }
            return;
        }

        public void SetDoorStatus(string who, bool isInternalDoorOpened, bool isOutsideDoorOpened)
        {
            var internalDoor = GridTerminalSystem.GetBlockWithName($"{who}-home-internal-door") as IMyDoor;
            if (isInternalDoorOpened)
            {
                if (internalDoor.Status == DoorStatus.Closed || internalDoor.Status == DoorStatus.Closing)
                {
                    internalDoor.OpenDoor();
                }
            }
            else
            {
                if (internalDoor.Status == DoorStatus.Open || internalDoor.Status == DoorStatus.Opening)
                {
                    internalDoor.CloseDoor();
                }
            }
            var outsideDoor = GridTerminalSystem.GetBlockWithName($"{who}-home-outside-door") as IMyDoor;
            if (isOutsideDoorOpened)
            {
                if (outsideDoor.Status == DoorStatus.Closed || outsideDoor.Status == DoorStatus.Closing)
                {
                    outsideDoor.OpenDoor();
                }
            }
            else
            {
                if (outsideDoor.Status == DoorStatus.Open || outsideDoor.Status == DoorStatus.Opening)
                {
                    outsideDoor.CloseDoor();
                }
            }
        }
    }
}

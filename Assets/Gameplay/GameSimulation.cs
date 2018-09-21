using System;
using System.Collections.Generic;
using System.Linq;
using Navigation;
using UnityEngine;

namespace Gameplay
{
    public class GameSimulation
    {
        public List<PlayerInformation> Players { get; private set; }
        private Dictionary<Guid, int> playerIdToId = new Dictionary<Guid, int>();
        private Scheduler scheduler;
        private int lastUnitId = 0;
        public int CurrentFrame { get; private set; }
        private Map map;


        public GameSimulation(Map map, List<PlayerInformation> players, GameConfiguration startConfiguration)
        {
            this.map = map;
            Players = players;
            for (int i = 0; i < players.Count; i++)
            {
                playerIdToId.Add(players[i].PlayerId, i);
            }

            InitializeStartingUnits(players, startConfiguration);

            scheduler = new Scheduler(players.Count);
        }

        private void InitializeStartingUnits(List<PlayerInformation> players, GameConfiguration startConfiguration)
        {
            for (int pIdx = 0; pIdx < players.Count; pIdx++)
            {
                for (int i = 0; i < startConfiguration.WorkerCount; i++)
                {
                    int unitId = GetUnitId();
                    map.AllUnits.Add(new Unit(map, unitId, new DeterministicVector2(unitId, 0), pIdx));
                }
            }
        }

        // TODO: Make Unit Management class or so
        private int GetUnitId()
        {
            return ++lastUnitId;
        }

        public void ScheduelePlayerCommands(Guid playerId, PlayerCommand playerCommand, int frameNumber)
        {
            if (!playerIdToId.ContainsKey(playerId))
                return;

            scheduler.Schedule(playerIdToId[playerId], playerCommand, frameNumber);
        }

        public bool SimulateFrame()
        {
            var frame = scheduler.GetFrame(CurrentFrame);
            if (frame.IsFrameReady)
            {

                foreach (var cmd in frame.GetPlayerCommands())
                {
                    ExecuteCommand(cmd);
                }
                map.UpdateUnits();

                CurrentFrame++;

                return true;
            }
            else
            {
                Debug.Log("Frame " + CurrentFrame + " wasn't ready.");
            }

            return false;
        }

        // TODO: Command Pattern or chain of responsibility or so...
        private void ExecuteCommand(KeyValuePair<int, PlayerCommand> cmd)
        {
            if (cmd.Value == null)
                return;

            var affectedUnits = GetUnitsForPlayer(cmd.Key, cmd.Value.UnitIds);
            switch (cmd.Value.Type)
            {
                case CommandType.Move:
                    foreach (var unit in affectedUnits)
                    {
                        unit.MoveTo(cmd.Value.Target.ToVector2XZ());
                    }
                    break;
                case CommandType.Idle:
                    foreach (var unit in affectedUnits)
                    {
                        unit.Idle();
                    }
                    break;
                default:
                    throw new InvalidOperationException("Command of type " + cmd.Value.Type + " is not known.");
            }
        }

        private List<Unit> GetUnitsForPlayer(int playerId, List<int> unitIds)
        {
            return map.AllUnits.Where(x => x.PlayerId == playerId && unitIds.Contains(x.Id)).ToList();
        }
    }
}
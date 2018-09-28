using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Assets.Navigation.AStar;
using ClipperLib;
using Gameplay;
using LibTessDotNet;
using Navigation;
using Navigation.DeterministicMath;
using UnityEditor;
using UnityEngine;

namespace Components.Debug
{
    [RequireComponent(typeof(MapComponent))]
    public class GameSimulationComponent : MonoBehaviour
    {
        public GameObject UnitPrefab;
        private GameSimulation gameSimulation = null;
        private MapComponent mapComponent;
        private Dictionary<int, UnitComponent> units = new Dictionary<int, UnitComponent>();

        // TODO: Get this by matchmaking
        public List<PlayerInformation> Players = new List<PlayerInformation>
        {
            new PlayerInformation()
            {
                Name = "Player 1",
                PlayerId = Guid.NewGuid()
            },
            new PlayerInformation()
            {
                Name = "Player 2",
                PlayerId = Guid.NewGuid()
            }
        };

        // TODO: comes from networking
        private Dictionary<int, PlayerCommand> currentCommands = new Dictionary<int, PlayerCommand>();

        public void Start()
        {
            this.mapComponent = this.GetComponent<MapComponent>();
            currentCommands = new Dictionary<int, PlayerCommand>();
            for (int i = 0; i < Players.Count; i++)
            {
                currentCommands.Add(i, null);
            }
        }
        private void MoveAllUnit(CommandType type)
        {
            var mainCam = GameObject.FindObjectOfType<Camera>();
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                currentCommands[0] = new PlayerCommand()
                {
                    Target = new DeterministicVector3(new DeterministicFloat(hitInfo.point.x),
                        new DeterministicFloat(hitInfo.point.y), new DeterministicFloat(hitInfo.point.z)),
                    Type = type,
                    UnitIds = this.mapComponent.Map.AllUnits.ConvertAll(x => x.Id).ToList()
                };
            }
        }

        public void Update()
        {
            if (Input.GetMouseButtonUp(1))
            {
                MoveAllUnit(CommandType.Move);
            }
            if (Input.GetMouseButtonUp(0))
            {
                MoveAllUnit(CommandType.Idle);
            }

        }

        public void FixedUpdate()
        {
            if (this.mapComponent.Map == null)
            {
                return;
            }

            if (this.gameSimulation == null)
            {
                gameSimulation = new GameSimulation(this.mapComponent.Map, Players,
                new GameConfiguration()
                {
                    WorkerCount = 20
                });

                foreach (var player in Players)
                {
                    this.gameSimulation.ScheduelePlayerCommands(player.PlayerId, null, 0);
                    this.gameSimulation.ScheduelePlayerCommands(player.PlayerId, null, 1);
                }
            }

            for (var i = 0; i < Players.Count; i++)
            {
                this.gameSimulation.ScheduelePlayerCommands(Players[i].PlayerId, currentCommands[i], this.gameSimulation.CurrentFrame + 2);
                currentCommands[i] = null;
            }

            this.gameSimulation.SimulateFrame();

            SpawnNewUnits();
        }

        private void SpawnNewUnits()
        {
            for (int i = this.mapComponent.Map.AllUnits.Count - 1; i >= 0; i--)
            {
                int unitId = this.mapComponent.Map.AllUnits[i].Id;
                if (units.ContainsKey(unitId))
                {
                    break;
                }

                var unitGo = GameObject.Instantiate(UnitPrefab);
                var unitComponent = unitGo.GetComponent<UnitComponent>();
                unitComponent.Unit = this.mapComponent.Map.AllUnits[i];
                units.Add(unitId, unitComponent);
            }
        }
    }
}
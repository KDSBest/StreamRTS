using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Assets.Navigation.AStar;
using Navigation;
using Navigation.DeterministicMath;
using Steering;
using UnityEngine;

namespace Gameplay
{
    public class Unit
    {
        public int Id = 0;
        public int PlayerId = -1;
        public DeterministicVector2 Position { get; set; }
        public DeterministicVector2 OldPosition = new DeterministicVector2();
        public DeterministicVector2 LastSteering = new DeterministicVector2();
        public DeterministicFloat Speed = new DeterministicFloat(0.3);
        public DeterministicFloat FunnelSize = 1;

        public List<NavigationEdge> Path;

        public bool IsWalking
        {
            get { return Path != null; }
        }

        public int RecalculatePathAfterUpdates = 5;

        public List<Unit> Neighbours = new List<Unit>();

        // not allowed to use outside because this is not deterministic
        private DeterministicVector2 newPosition = new DeterministicVector2();

        private int recalculatePathUpdates = 0;

        private Map map;
        private DeterministicVector2 lastTo;

        private List<ISteerBehaviour> steeringBehaviours = new List<ISteerBehaviour>()
        {
            new SteerForPathBehaviour(),
            //new SteerAlignmentBehaviour(),
            //new SteeUnitCollisionAvoidanceBehaviour()
            //new SteerCoherenceBehaviour()
        };

        private List<ISteerBehaviour> steeringBehavioursAfterMove = new List<ISteerBehaviour>()
        {
            new SteerUnitCollisionResolveBehaviour(),
            new SteerUnitCollisionConstrainedEdgeBehaviour()
        };

        public Unit(Map map, int id, DeterministicVector2 startingPosition, int playerId)
        {
            this.Id = id;
            Position = startingPosition;
            OldPosition = startingPosition;
            PlayerId = playerId;
            this.newPosition = startingPosition;
            this.map = map;
        }

        public void Idle()
        {
            this.Path = null;
        }

        public void MoveTo(DeterministicVector2 moveTarget)
        {
            if (map.Pathfinding != null)
            {
                var to = new DeterministicVector2(moveTarget);

                if (Path != null && Path.Count > 0)
                {
                    if (Path[Path.Count - 1].B == to)
                        return;
                }

                RecalculatePath(to);
            }
        }

        public void OnPathRecalculationNeeded()
        {
            if (this.Path == null)
                return;

            RecalculatePath(lastTo);
        }

        private void RecalculatePath(DeterministicVector2 to)
        {
            recalculatePathUpdates = 0;

            this.Path = null;
            var path = map.Pathfinding.CalculatePath(new DeterministicVector2(Position), to);
            lastTo = to;
            if (path != null)
            {
                this.Path = new List<NavigationEdge>();

                for (int i = 0; i < path.Count - 1; i++)
                {
                    this.Path.Add(new NavigationEdge(path[i].Position + path[i].ConstraintedEdgeNormal * FunnelSize, path[i + 1].Position + path[i + 1].ConstraintedEdgeNormal * FunnelSize));
                }
            }
        }

        private void Steering(List<ISteerBehaviour> behaviours, bool applySpeed)
        {
            DeterministicVector2 newSteering = new DeterministicVector2();

            foreach (var steeringBehaviour in behaviours)
            {
                newSteering += steeringBehaviour.Steer(this, Neighbours, Path, map);
            }

            if (applySpeed)
            {
                newSteering = newSteering.Normalize();
                newSteering *= Speed;
            }

            newPosition += newSteering;
        }

        public void CopyNewValuesAfterUpdate()
        {
            this.Position = newPosition;
            this.LastSteering = Position - OldPosition;
        }

        public void Update()
        {
            if (map == null)
                return;

            newPosition = new DeterministicVector2(this.Position);
            OldPosition = new DeterministicVector2(this.Position);

            Steering(steeringBehaviours, true);
            Steering(steeringBehavioursAfterMove, false);

            if (Path != null)
            {
                if ((this.Position - Path[Path.Count - 1].B).GetLength() <= FunnelSize)
                {
                    Idle();
                }
                recalculatePathUpdates++;

                if (recalculatePathUpdates >= RecalculatePathAfterUpdates)
                    RecalculatePath(lastTo);
            }
        }
    }
}

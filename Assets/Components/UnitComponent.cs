using System;
using System.Collections.Generic;
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
    public class UnitComponent : MonoBehaviour
    {
        public Transform DesiredPosition;
        public float FunnelSize = 1;
        public double Speed = 1;
        private static int newUnitId = 0;
        private Unit unit;

        public void Start()
        {
            newUnitId++;
            this.unit = new Unit(newUnitId);

            this.transform.position = new Vector3(this.transform.position.x, FunnelSize, this.transform.position.z);
            this.transform.localScale = new Vector3(FunnelSize * 2, FunnelSize * 2, FunnelSize * 2);
            unit.Speed = new DeterministicFloat(Speed);
            unit.FunnelSize = new DeterministicFloat(FunnelSize);
        }

        public void OnDrawGizmos()
        {
            if (this.unit == null)
                return;

            if (unit.Path != null)
            {
                Gizmos.color = Color.green;
                foreach (var edge in unit.Path)
                {
                    Gizmos.DrawLine(new Vector3(edge.A.X.ToFloat(), 0, edge.A.Y.ToFloat()), new Vector3(edge.B.X.ToFloat(), 0, edge.B.Y.ToFloat()));
                }
            }
        }

        public void FixedUpdate()
        {
            if (!unit.IsInitialized)
            {
                unit.Initialize(new DeterministicVector2(new DeterministicFloat(this.transform.position.x), new DeterministicFloat(this.transform.position.z)), GameObject.FindObjectOfType<MapComponent>().Map);
            }

            if (DesiredPosition != null)
            {
                unit.MoveTo(new DeterministicVector2(new DeterministicFloat(this.DesiredPosition.position.x), new DeterministicFloat(this.DesiredPosition.position.z)));
                DesiredPosition = null;
            }

            this.transform.position = new Vector3(unit.Position.X.ToFloat(), this.transform.position.y, unit.Position.Y.ToFloat());
        }
    }
}
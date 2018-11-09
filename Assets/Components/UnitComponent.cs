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
        private const float lerpSpeed = 10;
        public Unit Unit;
        private Animator animator;

        public GameObject Probe;
        public GameObject Stamp;

        public bool IsEnemyUnit
        {
            // TODO you are not always player one later on
            get { return Unit.PlayerId > 0; }
        }

        public void Start()
        {
            this.animator = GetComponent<Animator>();
            if (IsEnemyUnit)
            {
                var mats = Probe.GetComponent<Renderer>().materials;
                mats[mats.Length - 1].color = Color.blue;

                Stamp.SetActive(false);
            }
        }

        public void OnDrawGizmos()
        {
            if (this.Unit == null)
                return;

            if (Unit.Path != null)
            {
                Gizmos.color = Color.green;
                foreach (var edge in Unit.Path)
                {
                    Gizmos.DrawLine(new Vector3(edge.A.X.ToFloat(), 0, edge.A.Y.ToFloat()), new Vector3(edge.B.X.ToFloat(), 0, edge.B.Y.ToFloat()));
                }
            }
        }

        public void Update()
        {
            if (this.Unit == null)
                return;

            this.animator.SetBool("IsWalking", this.Unit.IsWalking);

            this.transform.LookAt(new Vector3(Unit.Position.X.ToFloat(), this.transform.position.y, Unit.Position.Y.ToFloat()), Vector3.up);

            var heightRay = new Ray(new Vector3(Unit.Position.X.ToFloat(), 1000.0f, Unit.Position.Y.ToFloat()), Vector3.down);
            RaycastHit hitInfo = new RaycastHit();
            float height = 0;
            if (Physics.Raycast(heightRay, out hitInfo, float.MaxValue, LayerMask.GetMask("Heightmap")))
            {
                height = hitInfo.point.y;
            }

            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(Unit.Position.X.ToFloat(), height, Unit.Position.Y.ToFloat()), lerpSpeed * Time.deltaTime);

        }
    }
}
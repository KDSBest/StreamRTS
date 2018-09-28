﻿using System;
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

        public void Start()
        {
            this.animator = GetComponent<Animator>();
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
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(Unit.Position.X.ToFloat(), this.transform.position.y, Unit.Position.Y.ToFloat()), lerpSpeed * Time.deltaTime);

        }
    }
}
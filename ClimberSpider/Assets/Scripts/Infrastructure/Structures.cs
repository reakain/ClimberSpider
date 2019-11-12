using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{

    // A typical error function to minimise
    public delegate float ErrorFunction(Vector3 target, float[] solution);

    public struct PositionRotation
    {
        public Vector3 position;
        public Quaternion rotation;

        public PositionRotation(Vector3 position, Quaternion rotation)
        {
            this.position = new Vector3(position.x, position.y, position.z);
            this.rotation = new Quaternion(rotation.x, rotation.y,rotation.z, rotation.w);
        }

        // PositionRotation to Vector3
        public static implicit operator Vector3(PositionRotation pr)
        {
            return pr.position;
        }
        // PositionRotation to Quaternion
        public static implicit operator Quaternion(PositionRotation pr)
        {
            return pr.rotation;
        }

        public static PositionRotation operator +(PositionRotation pr, Vector3 v)
        {
            return new PositionRotation(pr.position + v,pr.rotation);
        }

        public static PositionRotation operator -(PositionRotation pr, Vector3 v)
        {
            return new PositionRotation(pr.position - v, pr.rotation);
        }

        public override string ToString()
        {
            return "Position " + position.ToString() + " and Rotation " + rotation.ToString();
        }
    }

    public struct VertexNormal
    {
        public Vector3 vertex { get; private set; }
        public Vector3 normal { get; private set; }

        public VertexNormal(Vector3 vertex, Vector3 normal)
        {
            this.vertex = new Vector3(vertex.x, vertex.y, vertex.z);
            this.normal = new Vector3(normal.x, normal.y, normal.z);
        }
    }


    public struct SolutionPath
    {
        Solution[] solutionpath;

        public SolutionPath(Solution[] solutionpath)
        {
            this.solutionpath = new Solution[solutionpath.Length];
            for (int i = 0; i < solutionpath.Length; i++)
            {
                this.solutionpath[i] = solutionpath[i];
            }
        }
    }
}
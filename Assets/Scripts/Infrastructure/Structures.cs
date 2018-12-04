using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{

    // A typical error function to minimise
    public delegate float ErrorFunction(PositionRotation target, float[] solution);

    public struct PositionRotation
    {
        Vector3 position;
        Quaternion rotation;

        public PositionRotation(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
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

        public override string ToString()
        {
            return position.ToString();
        }
    }
}
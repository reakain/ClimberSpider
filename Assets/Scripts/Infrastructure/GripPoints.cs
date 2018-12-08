using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class GripPoints
    {
        private VertexNormal[] touchPoints = null;
        public GripPoints()
        {

        }

        public GripPoints(VertexNormal[] points)
        {
            SetGrip(points);
        }

        public GripPoints(GripPoints point)
        {
            SetGrip(point.GetGrip());
        }

        public void SetGrip(VertexNormal[] points)
        {
            touchPoints = new VertexNormal[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                touchPoints[i] = points[i];
            }
        }

        public VertexNormal[] GetGrip()
        {
            VertexNormal[] outPoint = new VertexNormal[touchPoints.Length];
            for (int i = 0; i < touchPoints.Length; i++)
            {
                outPoint[i] = touchPoints[i];
            }
            return outPoint;
        }
    }
}
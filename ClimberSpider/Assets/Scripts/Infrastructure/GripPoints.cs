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

        public VertexNormal[] ClampPoints(VertexNormal[] positions, float radius, float angleDelta)
        {
            if (positions.Length != touchPoints.Length)
            {
                return null;
            }

            var newPoints = new VertexNormal[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                // https://answers.unity.com/questions/1309521/how-to-keep-an-object-within-a-circlesphere-radius.html
                // Get vertex in range
                var newVert = positions[i].vertex;
                float distance = Vector3.Distance(newVert, touchPoints[i].vertex); //distance from ~green object~ to *black circle*

                if (distance > radius) //If the distance is less than the radius, it is already within the circle.
                {
                    Vector3 fromOriginToObject = newVert - touchPoints[i].vertex; //~GreenPosition~ - *BlackCenter*
                    fromOriginToObject *= radius / distance; //Multiply by radius //Divide by Distance
                    newVert = touchPoints[i].vertex + fromOriginToObject; //*BlackCenter* + all that Math
                }

                // Get normal within tolerance
                var newNorm = Vector3.RotateTowards(touchPoints[i].normal, positions[i].normal, angleDelta, 0.0f);
                newNorm.Normalize();
                newPoints[i] = new VertexNormal(newVert, newNorm);
            }
            return newPoints;
        }
    }
}
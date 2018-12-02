using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics.Analysis;

namespace SpiderBot
{
    public class GraspRegion : MonoBehaviour {

        public List<Vector3> GoalPoints;

        private double e1;
        private double e2;

        Collider m_ObjectCollider;

        [ReadOnly]
        public Tree GoalTree = new Tree();

        public Hand HandObject;

        Vector3[] PointCloud;

        // Use this for initialization
        void Start() {
            //Fetch the GameObject's collider (make sure they have a Collider component)
            m_ObjectCollider = gameObject.GetComponent<Collider>();
            //Output the GameObject's Collider Bound extents
            Debug.Log("extents : " + m_ObjectCollider.bounds.extents);
            Debug.Log("minimum : " + m_ObjectCollider.bounds.min);
            Debug.Log("maximum : " + m_ObjectCollider.bounds.max);
            DefineGoalRegion();
        }

        // Update is called once per frame
        void Update() {
            if (HandObject == null)
            {
                return;
            }

            FindGoal();
        }

        Vector3 GetSuperEllipsoidPoint(float a1, float a2, float a3, double eta, double w)
        {
            var x = Convert.ToSingle(a1 * Math.Pow(Math.Cos(eta), e1) * Math.Pow(Math.Cos(w), e2));
            var y = Convert.ToSingle(a2 * Math.Pow(Math.Cos(eta), e1) * Math.Pow(Math.Sin(w), e2));
            var z = Convert.ToSingle(a3 * Math.Pow(Math.Sin(eta), e1));

            return new Vector3() { x = x, y = y, z = z };
        }

        void GetPointCloud()
        {
            float fingerX = 0.6f;
            float fingerY = 0.6f;
            float fingerZ = 0.6f;
            Vector3[] PointCloud = GetComponent<MeshFilter>().mesh.vertices;
            for (var i = 0; i < PointCloud.Length; i++)
            {
                var vertex = PointCloud[i];
                vertex.x = vertex.x * fingerX;
                vertex.y = vertex.y * fingerY;
                vertex.z = vertex.z * fingerZ;
                PointCloud[i] = vertex;
            }

            //mesh.SetIndices(mesh.GetIndices(0), MeshTopology.Points, 0);
            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();
        }

        void DefineGoalRegion()
        {
            DefineShapeParameters();

            var aMin = m_ObjectCollider.bounds.min - new Vector3() { x = 0.6f, y = 0.6f, z = 0.6f };
            var aMax = m_ObjectCollider.bounds.max + new Vector3() { x = 0.6f, y = 0.6f, z = 0.6f };
            var aN = (aMax - aMin) / 100;

            double etaN = Math.PI / 20;

            double wMin = 0;
            double wMax = 1;
            double wN = (wMax - wMin) / 100;
            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();

            /*
            for (float i = aMin.x; i <= aMax.x; i += aN.x)
            {
                for (float j = aMin.y; j <= aMax.y; j += aN.y)
                {
                    for (float k = aMin.z; k <= aMax.z; k += aN.z)
                    {
                        for (double eta = -Math.PI/2; eta <= Math.PI/2; eta += etaN)
                        {
                            for (double w = wMin; w <= wMax; w += wN)
                            {
                                GoalPoints.Add(GetSuperEllipsoidPoint(i, j, k, eta, w));
                            }
                        }
                    }
                }
            }
            */
        }

        void DefineShapeParameters()
        {
            // Gradient Descent in here to pick e1 and e2
            e1 = 1;
            e2 = 1;
        }

        void FindGoal()
        {
            SampleHandPosition();
            VerifyGoal();
        }

        void SampleHandPosition()
        {
            // Get a grab pose
        }

        void VerifyGoal()
        {
            if (GoalTree.Count == 0)
            {
                GoalTree.Add(new Node(new Configuration(transform.position, transform.rotation, HandObject.FingerList), null, false, true));
            }
            //Check if it's a good grab pose
            //Check which regions it can touch without collision
        }
    }
}
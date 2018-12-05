﻿using System;
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
        public ArmPlanner Arm;

        public Wrist HandObject;

        private bool getGrasps = false;

        Vector3[] PointCloud;

        // Use this for initialization
        void Start() {
            //Fetch the GameObject's collider (make sure they have a Collider component)
            m_ObjectCollider = gameObject.GetComponent<Collider>();

            DefineGoalRegion();
        }

        public void Connect(ArmPlanner arm)
        {
            Arm = arm;
            HandObject = Arm.HandObject;
            getGrasps = true;
        }

        public void Disconnect()
        {
            getGrasps = false;
            Arm = null;
            HandObject = null;
        }

        public bool IsConnected(ArmPlanner arm)
        {
            if (getGrasps && Arm != null && Arm == arm && HandObject != null && HandObject == arm.HandObject)
            {
                return true;
            }
            return false;
        }

        // Update is called once per frame
        void Update() {
            if (!getGrasps || Arm == null || HandObject == null)
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

            // https://arxiv.org/ftp/arxiv/papers/1210/1210.7463.pdf
            //mesh.SetIndices(mesh.GetIndices(0), MeshTopology.Points, 0);
            /*Vector3[] mean = PointCloud.Mean();
			Vector3[] PointCloudAdjust = PointCloud.Subtract(mean);
			Vector3[] cov = PointCloudAdjust.Covariance();
			
			var evd = new EigenvalueDecompostion(cov);
			Vector3[] eigenvalues = evd.RealEigenvalues;
			Vector3[] eigenvectors = evd.Eigenvectors;
			
			eigenvectors = Matrix.Sort(eigenvalues, eigenvectors, new GeneralComparer(ComparerDirection.Descending, true));
			*/
            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();
			pca.Overwrite = true;
			//pca.Compute();
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
            if (Arm.GoalTree.Count == 0)
            {
                Arm.AddGoalNode(new Node(new Configuration(transform.position, transform.rotation, HandObject.FingerList), null, true));
            }
            //Check if it's a good grab pose
            //Check which regions it can touch without collision
        }
    }
}
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Accord.Statistics.Analysis;

/*
 * To-do list:
 * 
 * get vertice list
 * get grasp regions
 * start defining goal poses from grasp regions
 * IK solver calls to build reverse RRT?
 * 
 */

namespace SpiderBot
{
    public class GraspRegion : MonoBehaviour {

        public List<Vector3> GoalPoints;

        private float e1;
        private float e2;

        Collider m_ObjectCollider;

        [ReadOnly]
        public ArmPlanner Arm;

        public Wrist HandObject;

        private bool getGrasps = false;

        private float RegionSizing = 0;

        private List<Mesh> RegionList = null;

        Vector3[] PointCloud;

        List<Vector3> UnusedCloud;

        // Use this for initialization
        void Start() {
            //Fetch the GameObject's collider (make sure they have a Collider component)
            m_ObjectCollider = gameObject.GetComponent<Collider>();

            GetPointCloud();
        }

        public void Connect(ArmPlanner arm)
        {
            Arm = arm;
            HandObject = Arm.HandObject;
            getGrasps = true;
            SetRegionSizing();
        }

        public void Disconnect()
        {
            getGrasps = false;
            Arm = null;
            HandObject = null;
            RegionList = null;
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
            if (RegionList == null)
                AddNewRegions();
            FindGoal();
        }

        void AddNewRegions()
        {
            //SetRegionSizing();

            Vector3 point = UnusedCloud[Mathf.FloorToInt(Random.Range(0, UnusedCloud.Count - 1))];
            UnusedCloud.Remove(point);
            Vector3 otherCorner = UnusedCloud.Find(newpoint => Vector3.Distance(newpoint, point) <= RegionSizing + RegionSizing*0.1f && Vector3.Distance(newpoint,point) >= RegionSizing - RegionSizing*0.1f);

            
        }

        void SetRegionSizing()
        {
            RegionSizing = HandObject.GetComponentInChildren<FingerPad>().GetComponent<MeshCollider>().bounds.size.magnitude;
        }

        Vector3 GetSuperEllipsoidPoint(float a1, float a2, float a3, float eta, float w)
        {
            var x = a1 * Mathf.Pow(Mathf.Cos(eta), e1) * Mathf.Pow(Mathf.Cos(w), e2);
            var y = a2 * Mathf.Pow(Mathf.Cos(eta), e1) * Mathf.Pow(Mathf.Sin(w), e2);
            var z = a3 * Mathf.Pow(Mathf.Sin(eta), e1);

            return new Vector3() { x = x, y = y, z = z };
        }

        void GetPointCloud()
        {
            float fingerX = 0.0f;
            float fingerY = 0.0f;
            float fingerZ = 0.0f;
            PointCloud = GetComponent<MeshFilter>().mesh.vertices;
            UnusedCloud = new List<Vector3>();
            for (var i = 0; i < PointCloud.Length; i++)
            {
                var vertex = PointCloud[i];
                vertex.x = vertex.x * fingerX;
                vertex.y = vertex.y * fingerY;
                vertex.z = vertex.z * fingerZ;
                PointCloud[i] = vertex;
                UnusedCloud.Add(vertex);
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
            //PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();
			//pca.Overwrite = true;
			//pca.Compute();
        }

        void DefineGoalRegion()
        {
            DefineShapeParameters();

            var aMin = m_ObjectCollider.bounds.min - new Vector3() { x = 0.6f, y = 0.6f, z = 0.6f };
            var aMax = m_ObjectCollider.bounds.max + new Vector3() { x = 0.6f, y = 0.6f, z = 0.6f };
            var aN = (aMax - aMin) / 100;

            double etaN = Mathf.PI / 20;

            double wMin = 0;
            double wMax = 1;
            double wN = (wMax - wMin) / 100;
            //PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();

            /* THIS LOOP CRASHES EVERYTHING
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
            // Gradient Descent in here to pick e1 and e2?
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
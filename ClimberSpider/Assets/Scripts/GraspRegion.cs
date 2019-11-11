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

        private List<VertexNormal> RegionList = null;
        private List<VertexNormal> UnusedCloud = null;

        private List<GripPoints> PossibleHoldPoints = null;

        private float fingerSpace = 0;

        public int meshCount;


        VertexNormal[] PointCloud;

        

        // Use this for initialization
        void Start() {
            //Fetch the GameObject's collider (make sure they have a Collider component)
            m_ObjectCollider = gameObject.GetComponent<Collider>();
            meshCount = GetComponent<MeshCollider>().sharedMesh.vertexCount;

            //GetPointCloud();
            GetRegionList();
        }

        public void Connect(ArmPlanner arm)
        {
            Arm = arm;
            HandObject = Arm.HandObject;
            getGrasps = true;
            SetRegionSizing();
            GetAllGripPoints();
        }

        public void Disconnect()
        {
            getGrasps = false;
            Arm = null;
            HandObject = null;
            PossibleHoldPoints = null;
            RegionSizing = 0;
            fingerSpace = 0;
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
            if (!getGrasps || Arm == null || HandObject == null || RegionSizing == 0)
            {
                return;
            }

            var p = Random.value;
            if (p < 0.2) // Expand the tree
            {
                //ExpandTree(ArmTree, GoalTree);
            }
            else // Find a goal pose
            {
                FindGoal();
            }
        }

        void GetRegionList()
        {
            Vector3[] normals = GetComponent<MeshFilter>().mesh.normals;
            Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
            RegionList = new List<VertexNormal>();
            for (var i = 0; i < vertices.Length; i++)
            {
                RegionList.Add(new VertexNormal(normals[i], vertices[i]));
            }
            Debug.Log("Got " + RegionList.Count + " regions in " + gameObject.name);
        }

        void SetRegionSizing()
        {
            RegionSizing = HandObject.GetComponentInChildren<FingerPad>().GetComponent<MeshCollider>().bounds.size.magnitude;
        }

        void SetFingerSpacing()
        {
            var max = GetComponent<MeshCollider>().bounds.max;
            var min = GetComponent<MeshCollider>().bounds.min;
            fingerSpace = Vector3.Distance(max,min) / HandObject.FingerList.Length;
        }

        void ResetCloud()
        {
            UnusedCloud = new List<VertexNormal>();
            foreach (var region in RegionList)
            {
                UnusedCloud.Add(new VertexNormal(region.vertex, region.normal));
            }
        }

        void GetAllGripPoints()
        {
            ResetCloud();
            SetFingerSpacing();
            int i = 0;
            PossibleHoldPoints = new List<GripPoints>();
            while (UnusedCloud.Count > HandObject.FingerList.Length && i < 100)
            {
                if( !GetNewGrip())
                {
                    i++;
                }
            }
            Debug.Log("Got " + PossibleHoldPoints.Count + " Grip point sets!");
        }

        bool GetNewGrip()
        {
            VertexNormal[] pads = new VertexNormal[HandObject.FingerList.Length];
            pads[0] = UnusedCloud[Mathf.RoundToInt(Random.Range(0, UnusedCloud.Count - 1))];
            // Get a grab pose
            var startList = GetSpacedPad(RegionList, pads[0]);
            try
            {
                for (int i = 1; i < HandObject.FingerList.Length; i++)
                {
                    if (startList == null)
                    {
                        return false;
                    }
                    pads[i] = startList.Find(pad => pad.vertex != pads[i - 1].vertex && pad.normal != pads[i - 1].normal);
                    startList = GetSpacedPad(startList, pads[i]);
                }
                for (int i = 0; i < pads.Length; i++)
                {
                    UnusedCloud.Remove(pads[i]);
                }

                PossibleHoldPoints.Add(new GripPoints(pads));
                return true;
            }
            catch
            {
                return false;
            }
        }

        List<VertexNormal> GetSpacedPad(List<VertexNormal> padList, VertexNormal pad)
        {
            try
            {
               var list = padList.FindAll(newpoint => Vector3.Distance(newpoint.vertex, pad.vertex) <= (fingerSpace + (fingerSpace * 0.1f)) &&
                        Vector3.Distance(newpoint.vertex, pad.vertex) >= (fingerSpace - (fingerSpace * 0.1f)));
                return list;
            }
            catch
            {
                return null;
            }
        }

        void FindGoal()
        {
            SampleHandPosition();
            VerifyGoal();
        }

        void SampleHandPosition()
        {
            GripPoints newGrip = new GripPoints(PossibleHoldPoints[Mathf.RoundToInt(Random.Range(0, PossibleHoldPoints.Count - 1))]);

            PositionRotation[] points = new PositionRotation[HandObject.FingerList.Length];

            for(int i = 0; i < HandObject.FingerList.Length; i++)
            {
                var pads = HandObject.FingerList[i].GetComponentsInChildren<FingerPad>();
                points[i] = pads[pads.Length - 1].JointPositionFromPad(newGrip.GetGrip()[i]);
            }
            // From touch points back calculate finger poses;
            // 
        }

        /*
        int NumberGripCombinations()
        {
            int combos = PossibleHoldPoints[0].GetGrip().Length;
            combos = Mathf.
            foreach (var finger in HandObject.FingerList)
            {
                var pads = finger.GetComponentsInChildren<FingerPad>();
                combos
            }
        }
        */




        void VerifyGoal()
        {
            if (Arm.GoalTree.Count == 0)
            {
                Arm.AddGoalNode(new Node(new Configuration(transform.position, transform.rotation, HandObject.FingerList), null, true));
            }
            //Check if it's a good grab pose
            //Check which regions it can touch without collision
        }

        // DEPRECATED
        /*
        Vector3 GetSuperEllipsoidPoint(float a1, float a2, float a3, float eta, float w)
        {
            var x = a1 * Mathf.Pow(Mathf.Cos(eta), e1) * Mathf.Pow(Mathf.Cos(w), e2);
            var y = a2 * Mathf.Pow(Mathf.Cos(eta), e1) * Mathf.Pow(Mathf.Sin(w), e2);
            var z = a3 * Mathf.Pow(Mathf.Sin(eta), e1);

            return new Vector3() { x = x, y = y, z = z };
        }

        void GetPointCloud()
        {
            Vector3[] normals = GetComponent<MeshFilter>().mesh.normals;
            Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
            PointCloud = new VertexNormal[vertices.Length];
            UnusedCloud = new List<VertexNormal>();
            for (var i = 0; i < PointCloud.Length; i++)
            {
                PointCloud[i] = new VertexNormal(vertices[i], normals[i]);
                UnusedCloud.Add(PointCloud[i]);
            }
        }
        */
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


        // DEPRECATED
        /*
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
            
        }
        */
        void DefineShapeParameters()
        {
            // Gradient Descent in here to pick e1 and e2?
            e1 = 1;
            e2 = 1;
        }


    }
}
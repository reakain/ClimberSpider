using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot {
    public class RRT : MonoBehaviour
    {
        [Header("Grabber")]
        public Wrist HandObject;
        [Header("Destination")]
        public GraspRegion Destination;
        [Space]

        private float Delta = 0;

        private Collider[] ArmColliderList;
        private ArmSolver armSolver;

        private bool doSearch = true;
        private bool isComplete = false;

        public GameObject target;

        public float xLow, xHigh, yLow, yHigh, zLow, zHigh;
        public float stepSize, maxIterations, timeStep;
        public float overlapRadius;

        public bool step = false;
        public bool disableStepping = false;

        public LayerMask obstacleMask;
        public LayerMask targetMask;

        private PointTree forwardTree;
        private PointTree backwardTree;
        private int counter = 0;
        private float timer = 0f;
        private bool goForward = true;
        private Stack<Vector3> pointPath;


        // Debug drawing
        public bool enableDrawing = true;
        public float lineWidth = 1f;
        public Material lineMat;
        public Color forwardColor;
        public Color backwardColor;


        private void Awake()
        {

        }
        
        private void Start()
        {
            pointPath = new Stack<Vector3>();

            ArmColliderList = GetComponentsInChildren<Collider>();
            armSolver = GetComponent<ArmSolver>();

            // Initialize distance tolerances
            Delta = Toolbox.Instance.GetConnectionDistance();

            if (HandObject == null)
            {
                HandObject = GetComponentInChildren<Wrist>();
            }

            InitializeTrees();
        }

        void InitializeTrees()
        {
            // Initialize forward tree RRT
            forwardTree = new PointTree(new Vector3(HandObject.transform.position.x, HandObject.transform.position.y, HandObject.transform.position.z), true);
            forwardTree.lineWidth = lineWidth;
            forwardTree.enableDrawing = enableDrawing;
            forwardTree.lineMat = lineMat;
            forwardTree.lineColor = forwardColor;

            // Initialize backward tree RRT
            backwardTree = new PointTree(new Vector3(Destination.transform.position.x, Destination.transform.position.y, Destination.transform.position.z), false);
            backwardTree.lineWidth = lineWidth;
            backwardTree.enableDrawing = enableDrawing;
            backwardTree.lineMat = lineMat;
            backwardTree.lineColor = backwardColor;

            isComplete = false;
        }

        private void Update()
        {
            if (Destination == null || HandObject == null)
            {
                return;
            }
            //if (!Destination.IsConnected(this))
            //{
            //    Destination.Connect(this);
            //}
            if(isComplete)
            {
                //Stack<Vector3> pointsCopy = new Stack<Vector3>(pointPath);
                pointPath.Pop();
                var startSoln = new List<float[]>
                {
                    //GetComponent<IKSolver>().GetStartingAngles()
                    new float[armSolver.GetHomeAngles().Length]
                };
                var movePath = GenerateSolutionStep(startSoln, armSolver.GetJointPose(startSoln), pointPath);
                if(movePath == null)
                {
                    Debug.Log("FAILED TO FIND SOLUTION");
                    InitializeTrees();
                    return;
                }
                GetComponent<ArmController>().StartSolutionRun(new Solution(movePath));
            }

            if(doSearch)
            {
                RRTSearch();
            }
        }

        List<float[]> GenerateSolutionStep(List<float[]> solution, Vector3[] jointPose, Stack<Vector3> pointsLeft)
        {
            var newPath = armSolver.TestPath(solution, jointPose, pointsLeft.Pop());
            if (newPath != null)
            {
                if (pointsLeft.Count > 0)
                {
                    newPath = GenerateSolutionStep(newPath, armSolver.GetJointPose(newPath), pointsLeft);
                }
            }
            return newPath;
        }

        public void StartSearch()
        {
            doSearch = true;
        }

        public void StopSearch()
        {
            doSearch = false;
        }

        public void RRTSearch()
        {
            if (counter < maxIterations && !forwardTree.IsComplete() && !backwardTree.IsComplete() && !isComplete)
            {
                Vector3 randPoint = GetRandomPoint();
                //Debug.Log("Random Point" + randPoint.ToString());
                PointNode near;
                if (goForward)
                {
                    near = forwardTree.GetClosestLeaf(randPoint);
                }
                else
                {
                    near = backwardTree.GetClosestLeaf(randPoint);
                }

                //Debug.Log("Nearest Node" + near.position.ToString());
                Vector3 targPoint;

                if (!IsCollidingObstacle(near.position, randPoint))
                {
                    Vector3 direction = randPoint - near.position;
                    float multiplier = (stepSize < direction.magnitude) ? stepSize : direction.magnitude;
                    PointNode other = new PointNode(near.position + (direction.normalized * multiplier));
                    PointNode otherTreeNode;
                    Vector3 treeTarg;
                    if (goForward)
                    {
                        otherTreeNode = backwardTree.GetClosestLeaf(other.position);
                        treeTarg = new Vector3(backwardTree.rootNode.position.x, backwardTree.rootNode.position.y, backwardTree.rootNode.position.z);
                    }
                    else
                    {
                        otherTreeNode = forwardTree.GetClosestLeaf(other.position);
                        treeTarg = new Vector3(forwardTree.rootNode.position.x, forwardTree.rootNode.position.y, forwardTree.rootNode.position.z);
                    }
                    if (IsReachedGoal(near.position, other.position, otherTreeNode.position, treeTarg, out targPoint))
                    {
                        if (targPoint == otherTreeNode.position)
                        {
                            other = GetCompositePath(near, otherTreeNode);
                            if (goForward)
                            {
                                forwardTree.AddFinalNode(ref other);
                                forwardTree.DrawCompletedPath();

                            }
                            else
                            {
                                backwardTree.AddFinalNode(ref other);
                                backwardTree.DrawCompletedPath();
                            }
                        }
                        else
                        {
                            direction = targPoint - near.position;
                            multiplier = (stepSize < direction.magnitude) ? stepSize : direction.magnitude;
                            other = new PointNode(near.position + (direction.normalized * multiplier));
                            if (goForward)
                            {
                                other = forwardTree.AddLeaf(ref near, ref other);
                                forwardTree.AddFinalNode(ref other);
                                forwardTree.DrawCompletedPath();
                            }
                            else
                            {
                                other = backwardTree.AddLeaf(ref near, ref other);
                                backwardTree.AddFinalNode(ref other);
                                backwardTree.DrawCompletedPath();
                            }
                        }

                        GetCompletePath(other);
                        if (!goForward)
                        {
                            pointPath = new Stack<Vector3>(pointPath.ToArray());
                        }
                        isComplete = true;
                        Debug.Log("PRINTING FINAL PATH");
                        foreach (Vector3 point in pointPath)
                        {
                            Debug.Log(point.ToString());
                        }
                    }
                    else
                    {
                        //Debug.Log("Node Added At " + other.position.ToString());
                        if (goForward)
                        {
                            forwardTree.AddLeaf(ref near, ref other);
                        }
                        else
                        {
                            backwardTree.AddLeaf(ref near, ref other);
                        }
                    }
                }

                if (isComplete)
                {

                }

                goForward = !goForward;
                counter++;
                timer = 0;
            }

        }

        public void SetNewObject(GraspRegion target)
        {
            if (Destination != null && Destination != target)
            {
                //Destination.Disconnect();
                Destination = null;
            }
            if (Destination == null)
            {
                Destination = target;
                //ArmTree = new Tree();
                //GoalTree = new Tree();
                //SolutionPathList = new SolutionList();
                //Destination.Connect(this);
                InitializeTrees();
            }
        }

        PointNode GetCompositePath(PointNode parent, PointNode child)
        {
            PointNode childNew = new PointNode(child.position, parent, goForward);
            if (goForward)
            {
                childNew = forwardTree.AddLeaf(ref parent, ref childNew);
            }
            else
            {
                childNew = backwardTree.AddLeaf(ref parent, ref childNew);
            }
            if(child.parent != null)
            {
                return GetCompositePath(childNew, child.parent);
            }
            return childNew;
        }

        bool IsCollidingObstacle(Vector3 from, Vector3 to)
        {
            RaycastHit hit;
            if (Physics.Raycast(from, (to - from).normalized, out hit, stepSize, obstacleMask))
            {
                if(hit.collider != null)
                {
                    return true;
                }
            }
            return false;
        }

        void GetCompletePath(PointNode node)
        {
            pointPath.Push(node.position);
            if (node.parent != null)
            {
                GetCompletePath(node.parent);
            }
        }

        bool IsReachedGoal(Vector3 from, Vector3 to, Vector3 closestOtherTree, Vector3 targetNode, out Vector3 targetPos)
        {
            targetPos = Vector3.zero;
            RaycastHit hit;
            if (Physics.Raycast(from, (to - from).normalized, out hit, stepSize, targetMask))
            {
                if (hit.collider != null && hit.collider.gameObject.transform.position == targetNode)
                {
                    targetPos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
                    return true;
                }
            }
            else
            {
                if(PointCircleCollision(to, closestOtherTree))
                {
                    targetPos = new Vector3(closestOtherTree.x, closestOtherTree.y, closestOtherTree.z);
                    return true;
                }
            }
            return false;
        }

        bool PointCircleCollision(Vector3 from, Vector3 to)
        {
            if (Vector3.Distance(from, to) < overlapRadius)
            {
                return true;
            }
            return false;
        }

        Vector3 GetRandomPoint()
        {
            return new Vector3(Random.Range(xLow, xHigh), Random.Range(yLow, yHigh), Random.Range(zLow,zHigh));
        }
    }
}
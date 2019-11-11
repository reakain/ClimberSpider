using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot {
    public class RRT : MonoBehaviour
    {
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
            forwardTree = new PointTree(new Vector3(transform.position.x, transform.position.y, transform.position.z), true);
            forwardTree.lineWidth = lineWidth;
            forwardTree.enableDrawing = enableDrawing;
            forwardTree.lineMat = lineMat;
            forwardTree.lineColor = forwardColor;

            backwardTree = new PointTree(new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z), false);
            backwardTree.lineWidth = lineWidth;
            backwardTree.enableDrawing = enableDrawing;
            backwardTree.lineMat = lineMat;
            backwardTree.lineColor = backwardColor;
        }

        private void Update()
        {
            //if (!disableStepping)
            //{
            //    if (step)
            //    {
            //        timer = timeStep + 1f;
            //        step = false;
            //    }
            //}
            //else
            //{
            //    timer += Time.deltaTime;
            //}

            if (/*timer > timeStep &&*/ counter < maxIterations && !forwardTree.IsComplete() && !backwardTree.IsComplete())
            {
                Vector3 randPoint = GetRandomPoint();
                Debug.Log("Random Point" + randPoint.ToString());
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

                if(!IsCollidingObstacle(near.position,randPoint))
                {
                    Vector3 direction = randPoint - near.position;
                    float multiplier = (stepSize < direction.magnitude) ? stepSize : direction.magnitude;
                    PointNode other = new PointNode(near.position + (direction.normalized * multiplier));
                    PointNode otherTreeNode;
                    Vector3 treeTarg;
                    if(goForward)
                    {
                        otherTreeNode = backwardTree.GetClosestLeaf(other.position);
                        treeTarg = new Vector3(backwardTree.rootNode.position.x, backwardTree.rootNode.position.y, backwardTree.rootNode.position.z);
                    }
                    else
                    {
                        otherTreeNode = forwardTree.GetClosestLeaf(other.position);
                        treeTarg = new Vector3(forwardTree.rootNode.position.x, forwardTree.rootNode.position.y, forwardTree.rootNode.position.z);
                    }
                    if (IsReachedGoal(near.position,other.position,otherTreeNode.position,treeTarg,out targPoint))
                    {
                        if (targPoint == otherTreeNode.position)
                        {
                            PointNode finalNode = GetCompositePath(near, otherTreeNode);
                            if(goForward)
                            {
                                forwardTree.AddFinalNode(ref finalNode);
                                forwardTree.DrawCompletedPath();
                            }
                            else
                            {
                                backwardTree.AddFinalNode(ref finalNode);
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
                    }
                    else
                    {
                        Debug.Log("Node Added At " + other.position.ToString());
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

                goForward = !goForward;

                //int CollisionInfo = IsColliding(near.position, randPoint, out targPoint);

                //if (CollisionInfo != -1)
                //{
                //    if (CollisionInfo == 1)
                //    {
                //        Debug.Log("COMPLETE");
                //        Vector3 direction = targPoint - near.position;
                //        float multiplier = (stepSize < direction.magnitude) ? stepSize : direction.magnitude;
                //        PointNode other = new PointNode(near.position + (direction.normalized * multiplier));
                //        Debug.Log("Node Added At " + other.position.ToString());
                //        other = forwardTree.AddLeaf(ref near, ref other);
                //        forwardTree.AddFinalNode(ref other);
                //        forwardTree.DrawCompletedPath();
                //    }
                //    else
                //    {
                //        Vector3 direction = randPoint - near.position;
                //        float multiplier = (stepSize < direction.magnitude) ? stepSize : direction.magnitude;
                //        PointNode other = new PointNode(near.position + (direction.normalized * multiplier));
                //        Debug.Log("Node Added At " + other.position.ToString());
                //        forwardTree.AddLeaf(ref near, ref other);
                //    }
                //}

               


                    counter++;
                timer = 0;
            }
        }

        int IsColliding(Vector3 from, Vector3 to, out Vector3 targPos)
        {
            int temp = 0;
            targPos = Vector3.zero;

            RaycastHit hit;
            if (Physics.Raycast(from, (to - from).normalized, out hit, stepSize))
            {
                if (hit.collider != null)
                {

                    if (hit.collider.gameObject.tag == "target")
                    {
                        temp = 1;
                        var targtrans = hit.collider.transform.position;
                        targPos = new Vector3(targtrans.x, targtrans.y, targtrans.z);
                    }
                    else
                    {
                        temp = -1;
                    }

                }
            }

            return temp;
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
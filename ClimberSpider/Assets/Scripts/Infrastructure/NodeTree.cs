using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpiderBot
{

    public class PointNode
    {
        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }
        public PointNode parent { get; private set; }

        public List<PointNode> children { get; private set; }

        public bool forwardNode { get; private set; }

        public PointNode(bool forward = true)
        {
            parent = null;
            children = new List<PointNode>();
            position = new Vector3(0, 0, 0);
            forwardNode = forward;
        }

        public PointNode(Vector3 v, bool forward = true)
        {
            parent = null;
            children = new List<PointNode>();
            position = new Vector3(v.x, v.y, v.z);
            forwardNode = forward;
        }

        public PointNode(Vector3 v, PointNode par, bool forward = true)
        {
            parent = par;
            children = new List<PointNode>();
            position = new Vector3(v.x, v.y, v.z);
            forwardNode = forward;
        }

        public void SetParent(PointNode par)
        {
            parent = par;
        }

        public void AddChild(ref PointNode child)
        {
            child.SetParent(this);
            children.Add(child);
        }

    }
    public class Node
    {
        public Node ParentNode { get; private set; }

        public Configuration Point { get; private set; }
        
        public bool IsGoal { get; private set; }
        private List<float[]> SolutionSteps;

        public Node(Configuration handConfiguration, Node parentNode = null, bool goal = false)
        {
            Point = handConfiguration.Clone();
            ParentNode = parentNode;
            IsGoal = goal;
        }

        public void AddSolutionSteps(List<float[]> solutionSteps)
        {
            SolutionSteps = new List<float[]>();
            for (int i = 0; i < solutionSteps.Count; i++)
            {
                float[] steps = new float[solutionSteps[i].Length];
                for (int j = 0; j < solutionSteps[i].Length; j++)
                {
                    steps[j] = solutionSteps[i][j];
                }
                SolutionSteps.Add(steps);
            }
        }

        public bool LinkToNode(Node node, bool overwrite = false)
        {
            if (ParentNode == null || overwrite == true)
            {
                ParentNode = node;
                return true;
            }
            return false;
        }

        public List<float[]> GetSolutionPath()
        {
            if (this.SolutionSteps == null) { return null; }
            List<float[]> solutionSteps = new List<float[]>();
            for (int i = 0; i < SolutionSteps.Count; i++)
            {
                float[] steps = new float[SolutionSteps[i].Length];
                for (int j = 0; j < SolutionSteps[i].Length; j++)
                {
                    steps[j] = SolutionSteps[i][j];
                }
                solutionSteps.Add(steps);
            }
            return solutionSteps;
        }

        public Node Clone()
        {
            Node copyNode = new Node(this.Point, this.ParentNode, this.IsGoal);
            if (this.SolutionSteps == null) { return copyNode; }

            copyNode.AddSolutionSteps(this.GetSolutionPath());
            return copyNode;
        }
    }

    public class Tree : List<Node>
    {
        public Node SampleFreeSpace()
        {
            Vector3 randPos = Random.insideUnitSphere * 8.7f;
            if (randPos.y >= 0)
            {
                //var node = this[Random.Range(0, Count - 1)];
                //var c = node.Point;
                var Delta = Toolbox.Instance.GetConnectionDistance();

                //Vector3 randPos = c.transform;
                //randPos += new Vector3(Random.Range(-Delta, Delta), Random.Range(-Delta, Delta), Random.Range(-Delta, Delta));

                Quaternion randRot = Random.rotationUniform;
                var best = FindClosest(randPos);
                var cNew = best.Point.MoveTowards(new Configuration(randPos, randRot, best.Point.FingerList));
                if (!PointInList(cNew))
                {
                    return (new Node(cNew, best));
                }
            }

            return null;
        }

        public bool PointInList(Configuration c)
        {
            foreach (var node in this)
            {
                if (node.Point == c)
                    return true;
            }
            return false;
        }

        public Node FindClosest(Vector3 c)
        {
            var best = this[0];
            foreach (var node in this)
            {
                if (Vector3.Distance(node.Point.transform, c) < Vector3.Distance(best.Point.transform, c))
                    best = node;
            }
            return best;
        }

        public Node FindClosest(Configuration c)
        {
            var best = this[0];
            foreach (var node in this)
            {
                if (c.Distance(node.Point) < c.Distance(best.Point))
                    best = node;
            }
            return best;
        }
    }

   public class PointTree
    {
        public PointNode rootNode { get; private set; }
        private bool complete = false;
        private PointNode finalNode;
        private int count = 0;
        bool forwardTree;

        // Debug drawing
        public float lineWidth = 1f;
        public bool enableDrawing = true;
        public Material lineMat;
        public Color lineColor;

        public void ViewParent(PointNode x)
        {
            Debug.Log("Parent is " + x.parent.position);
        }

        public PointTree(Vector3 pos, bool forward = true)
        {
            forwardTree = forward;
            rootNode = new PointNode(pos, forwardTree);
        }

        public bool IsComplete()
        {
            return complete;
        }

        public void AddFinalNode(ref PointNode k)
        {
            finalNode = k;

            complete = true;
        }

        public void CreateAndAdd(Vector3 pos)
        {
            PointNode nd = new PointNode(pos, forwardTree);
            PointNode k = GetClosestLeaf(nd.position);
            AddLeaf(ref k, ref nd);
        }
        public PointNode AddLeaf(ref PointNode parentLeaf, ref PointNode childLeaf)
        {
            parentLeaf.AddChild(ref childLeaf);
            if (enableDrawing)
            {

                DrawLine(childLeaf.position, parentLeaf.position, lineColor);
            }
            return childLeaf;
        }
        public PointNode GetClosestLeaf(Vector3 pos)
        {
            return FindClosestInChildren(rootNode, pos);
        }

        private PointNode FindClosestInChildren(PointNode x, Vector3 target)
        {
            PointNode closest = x, temp;
            float closestDistance = Vector3.Distance(x.position, target);
            float checkDistance = 0f;
            if(x.children.Count != 0)
            {
                foreach(PointNode child in x.children)
                {
                    temp = FindClosestInChildren(child, target);
                    checkDistance = Vector3.Distance(temp.position, target);
                    if(checkDistance < closestDistance)
                    {
                        closestDistance = checkDistance;
                        closest = temp;
                    }
                }
            }

            return closest;
        }

        public void DrawCompletedPath()
        {
            Debug.Log("Drawing Completed Non Recursive");
            DrawCompletedPath(finalNode);
            Debug.Log("LINES DRAWN:" + count.ToString());
        }
        private void DrawCompletedPath(PointNode x)
        {
            Debug.Log("Drawing Completed Recursive");
            if (!complete)
            {
                Debug.Log("Drawing Completed Recursive");
                return;
            }
            if (x.parent == null)
            {
                Debug.Log("Parent is Null");
                return;
            }

            count++;
            DrawLine(x.parent.position, x.position, Color.red);
            DrawCompletedPath(x.parent);
        }

        private void DrawLine(Vector3 _start, Vector3 _end, Color color)
        {
            Vector3 start = new Vector3(_start.x, _start.y, _start.z);
            Vector3 end = new Vector3(_end.x, _end.y, _end.z);

            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = lineMat;//new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.generateLightingData = true;
        }
    }

    /*
     * Algorithm BuildRRT
      Input: Initial configuration qinit, number of vertices in RRT K, incremental distance Δq)
      Output: RRT graph G

      G.init(qinit)
      for k = 1 to K
        qrand ← RAND_CONF()
        qnear ← NEAREST_VERTEX(qrand, G)
        qnew ← NEW_CONF(qnear, qrand, Δq)
        G.add_vertex(qnew)
        G.add_edge(qnear, qnew)
      return G
      */
    /*
  class Graph
  {
      public void Init(object qinit) { }
      public void AddEdge(object qnear, object qnew) { }
      public void AddVertex(object qnew) { }
  }

  class RRT
  {
      static object RAND_CONF() { return null; }
      static object NEAREST_VERTEX(object qrand, Graph g) { return null; }
      static object NEW_CONF(object qnear, object qrand, object deltaq) { return null; }

      static void Main(string[] args)
      {
          Graph g = new Graph();
          const int K = 1234;
          object deltaq = null;
          object qnear = null;
          object qnew = null;
          object qrand = null;
          for (int k = 1; k < K; k++)
          {
              qrand = RAND_CONF();
              qnear = NEAREST_VERTEX(qrand, g);
              qnew = NEW_CONF(qnear, qrand, deltaq);
              g.AddVertex(qnew);
              g.AddEdge(qnear, qnew);
          }

          //Console.WriteLine("Done.");
          //Console.ReadLine();
      } 
  }*/
}
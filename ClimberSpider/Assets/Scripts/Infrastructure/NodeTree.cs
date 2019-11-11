using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpiderBot
{
    public class Node
    {
        public Configuration Point { get; private set; }
        public Node ParentNode { get; private set; }
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
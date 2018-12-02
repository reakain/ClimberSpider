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
        public bool IsStart { get; private set; }

        public Node(Configuration handConfiguration, Node parentNode = null, bool start = false, bool goal = false)
        {
            Point = handConfiguration;
            ParentNode = parentNode;
            IsStart = start;
            IsGoal = goal;
        }

        public bool LinkToNode(Node node)
        {
            if (ParentNode == null)
            {
                ParentNode = node;
                return true;
            }
            return false;
        }
    }

    public class Tree : List<Node>
    {
        public Node ConnectNode(Node newNode)
        {
            foreach (var node in this)
            {
                if (node.Point.Distance(newNode.Point) <= Toolbox.Instance.GetConnectionDistance())
                {
                    if (node.Point.Angle(newNode.Point) <= Toolbox.Instance.GetConnectionAngle())
                    {
                        return node ;
                    }
                }
            }
            return null;
        }

        public void AddPathToList(Node newNode)
        {
            // Take your successful node and build the path and add it to a rated list
        }
    }

    public class Solution : LinkedList<Configuration>
    {
        public Solution(Node linkedNode, Node goalNode)
        {
            AddFirst(goalNode.Point);
            AddFirst(linkedNode.Point);

            Node parent = linkedNode.ParentNode;
            while (parent != null)
            {
                AddFirst(parent.Point);
                parent = parent.ParentNode;
            }
        }

        public override string ToString()
        {
            Vector3[] positions = new Vector3[this.Count];
            var i = 0;
            foreach (var node in this)
            {
                positions[i] = node.transform;
                i++;
            }
            return positions.ToString();
        }
    }

    public class SolutionList : List<Solution>
    {
        public bool AddSolutionIfExists(Node linkedNode, Tree goalTree)
        {
            foreach (var node in goalTree)
            {
                if (node.Point.Distance(linkedNode.Point) <= Toolbox.Instance.GetConnectionDistance())
                {
                    if (node.Point.Angle(linkedNode.Point) <= Toolbox.Instance.GetConnectionAngle())
                    {
                        Add(new Solution(linkedNode, node));
                        return true;
                    }
                }
            }
            return false;
        }

        public Solution ShortestPath()
        {
            var shortest = this[0];
            foreach (var soln in this)
            {
                if (soln.Count < shortest.Count)
                {
                    shortest = soln;
                }
            }
            return shortest;
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
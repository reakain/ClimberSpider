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
    }

    public class Tree : List<Node>
    {

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
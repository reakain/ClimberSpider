using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpiderBot
{
    public class Solution1 : LinkedList<float[]>
    {
        public Solution1()
        {

        }

        public Solution1(Node linkedNode, Node goalNode)
        {
            foreach (var solution in linkedNode.GetSolutionPath())
            {
                AddLast(solution);
            }
           /* var flip = false;
            // Build backward tree half
            AddFirst(goalNode.Point.);
            var parent = goalNode.ParentNode;
            while (parent != null)
            {
                AddLast(parent.Point);
                parent = parent.ParentNode;
            }

            // Build forward tree half
            AddFirst(linkedNode.Point);
            if (linkedNode.IsGoal)
                flip = true;

            parent = linkedNode.ParentNode;
            while (parent != null)
            {
                if (parent.IsGoal)
                {
                    flip = true;
                }
                AddFirst(parent.Point);
                parent = parent.ParentNode;
            }

            if (flip)
            {
                ReverseList();
            }
            */
        }

        public void Push(float[] point)
        {
            AddFirst(point);
        }

        public float[] Pop()
        {
            var point = First;
            RemoveFirst();
            return point.Value;
        }

        public bool IsEmpty()
        {
            if (Count == 0)
                return true;

            return false;
        }

        public void ReverseList()
        {
            var newList = new Solution1();
            while (!IsEmpty())
            {
                newList.Push(Pop());
            }

            while (!newList.IsEmpty())
            {
                Push(newList.Pop());
            }
        }

        public override string ToString()
        {
            string positions = "";
            foreach (var node in this)
            {
                positions += node.ToString() + "\n";
            }
            return positions.ToString();
        }
    }

    public class SolutionList : List<Solution1>
    {
        public Solution1 AddSolutionIfExists(Node linkedNode, Tree goalTree)
        {
            foreach (var node in goalTree)
            {
                if (node.Point.Distance(linkedNode.Point) <= Toolbox.Instance.GetConnectionDistance())
                {
                    if (node.Point.Angle(linkedNode.Point) <= Toolbox.Instance.GetConnectionAngle())
                    {
                        var soln = new Solution1(linkedNode, node);
                        Add(soln);
                        return soln;
                    }
                }
            }
            return null;
        }

        public Solution1 ShortestPath()
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
}

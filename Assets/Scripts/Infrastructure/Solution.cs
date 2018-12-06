using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpiderBot
{
    public struct SolutionStep
    {
        float[] solution;

        public SolutionStep(float[] solution)
        {
            this.solution = solution;
        }
    }

    public class Solution : LinkedList<float[]>
    {
        public Solution()
        {

        }

        public Solution(Node linkedNode, Node goalNode)
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
            var newList = new Solution();
            while (!IsEmpty())
            {
                newList.Push(Pop());
            }

            while (!newList.IsEmpty())
            {
                Push(newList.Pop());
            }
        }
		
		public bool HasCollision(Solution compare)
		{
			return true;
		}

        public override string ToString()
        {
            string positions = "";
            foreach (var node in this)
            {
                positions += "( ";
                foreach (var point in node)
                {
                    positions += point.ToString() + ", ";
                }
                positions += ")\n";
            }
            return positions.ToString();
        }
    }

    public class SolutionList : List<Solution>
    {
        public Solution AddSolutionIfExists(Node linkedNode, Tree goalTree)
        {
            foreach (var node in goalTree)
            {
                if (node.Point.Distance(linkedNode.Point) <= Toolbox.Instance.GetConnectionDistance())
                {
                    if (node.Point.Angle(linkedNode.Point) <= Toolbox.Instance.GetConnectionAngle())
                    {
                        var soln = new Solution(linkedNode, node);
                        Add(soln);
                        return soln;
                    }
                }
            }
            return null;
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
		

		public int ShortestLength()
        {
            var shortest = 0;
            foreach (var soln in this)
            {
                if (soln.Count < shortest)
                {
                    shortest = soln.Count;
                }
            }
            return shortest;
        }
		
		// Reads array of arm solution lists, finds all safe solns in own list versus these lists and returns that safe travel list
		public SolutionList GetCollisionFreeList(SolutionList[] checkList)
		{
			
			return null;
		}
		
		public Solution PopShortest()
		{
			Solution shortest = ShortestPath();
			this.Remove(shortest);
			return shortest;
		}
		
		
		public Solution LongestPath()
		{
			var longest = this[0];
			foreach (var soln in this)
            {
                if (soln.Count > longest.Count)
                {
                    longest = soln;
                }
            }
			return longest;
		}
		
		public int LongestLength()
		{
			int longest = 0;
			foreach (var soln in this)
            {
                if (soln.Count > longest)
                {
                    longest = soln.Count;
                }
            }
			return longest;
		}
    }
}

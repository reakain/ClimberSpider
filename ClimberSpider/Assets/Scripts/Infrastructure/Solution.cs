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
            this.solution = new float[solution.Length];
            for (int i = 0; i < solution.Length; i++)
            {
                this.solution[i] = solution[i];
            }
        }
    }

    public class Solution : LinkedList<float[]>
    {
        public Solution()
        {

        }

        public Solution(List<float[]> path)
        {
            foreach (float[] step in path)
            {
                float[] addSolution = new float[step.Length];
                for (int i = 0; i < step.Length; i++)
                {
                    addSolution[i] = step[i];
                }
                AddLast(addSolution);
            }
        }

        public Solution(Node linkedNode, Node goalNode)
        {
            foreach (var solution in linkedNode.GetSolutionPath())
            {
                float [] addSolution = new float[solution.Length];
                for (int i = 0; i < solution.Length; i++)
                {
                    addSolution[i] = solution[i];
                }
                AddLast(addSolution);
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
            float[] pushedPoint = new float[point.Length];
            for (int i = 0; i < point.Length; i++)
            {
                pushedPoint[i] = point[i];
            }
            AddFirst(pushedPoint);
        }

        public float[] Pop()
        {
            float [] poppedPoint = First.Value;
            RemoveFirst();

            float[] point = new float[poppedPoint.Length];
            for (int i = 0; i < poppedPoint.Length; i++)
            {
                point[i] = poppedPoint[i];
            }
            return point;
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

        public Solution Clone()
        {
            Solution copySoln = new Solution();
            foreach (var point in this)
            {
                var copypoint = new float[point.Length];
                for (int i = 0; i < point.Length; i++)
                {
                    copypoint[i] = point[i];
                }
                copySoln.AddLast(copypoint);
            }
            return copySoln;
        }
    }

    public class SolutionList : List<Solution>
    {
        public Solution AddSolutionIfExists(Node linkedNode, Tree goalTree)
        {
            foreach (var node in goalTree)
            {
                if (node.Point.Distance(linkedNode.Point) <= Toolbox.Instance.GetGoalDistance())
                {
                    if (node.Point.Angle(linkedNode.Point) <= Toolbox.Instance.GetConnectionAngle())
                    {
                        var soln = new Solution(linkedNode, node);
                        Add(soln);
                        return soln.Clone();
                    }
                }
            }
            return null;
        }

        public Solution ShortestPath()
        {
            var shortest = this[0].Clone();
            foreach (var soln in this)
            {
                if (soln.Count < shortest.Count)
                {
                    shortest = soln.Clone();
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
			var longest = this[0].Clone();
			foreach (var soln in this)
            {
                if (soln.Count > longest.Count)
                {
                    longest = soln.Clone();
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

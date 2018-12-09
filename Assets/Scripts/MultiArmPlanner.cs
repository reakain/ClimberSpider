using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * To-do list:
 * 
 * Build search call start
 * Build deadlock checks
 * Check for solns on update
 * if soln in each arm, check solns for deadlocks
 * if no deadlocks, stop search and move?
 * 
 */

namespace SpiderBot
{
    public class MultiArmPlanner : MonoBehaviour
    {

        private ArmPlanner[] Arms = null;
        //public float[] Solution = null;
		
		private SolutionList[] ArmSolutions = null;
		private Solution[] MovePath = null;

        private bool makingPlan = false;
        private bool newSolution = false;
        // Use this for initialization
        void Start()
        {
			Arms = GetComponentsInChildren<ArmPlanner>();
			ArmSolutions = new SolutionList[Arms.Length];
        }

        // Update is called once per frame
        void Update()
        {
			//if (Input.StartButton.IsDown && !makingPlan)
			//{
				//StartMultiRobotSearch();
			//}
			if (newSolution)
			{
				UpdateSolutionList();
				if (AllHaveSolutions())
					FindSafePath();
			}
        }
		
		void UpdateSolutionList()
		{
			ArmSolutions = new SolutionList[Arms.Length];
			for (int i = 0; i < Arms.Length; i++)
			{
				var armList = Arms[i].SolutionPathList;
				for (int j = 0; j < armList.Count; j++)
				{
					var armSoln = armList[j];
                    var soln = armSoln.Clone();
					ArmSolutions[i].Add(soln);
				}
			}
			newSolution = false;
		}

		bool AllHaveSolutions()
		{
			foreach (var solList in ArmSolutions)
			{
				if (solList.Count < 1)
					return false;
			}
			return true;
		}
		
		void FindSafePath()
		{
			bool hasPath = false;
			//int shortestSet = ;
			MovePath = new Solution[ArmSolutions.Length];
			while (!hasPath && ArmSolutions[0].Count > 0)
			{
                SolutionList[] shortFirst = new SolutionList[ArmSolutions.Length];
				shortFirst[0].Add(ArmSolutions[0].PopShortest());
				for (int i = 1; i < ArmSolutions.Length; i++)
				{
					
					shortFirst[i] = ArmSolutions[i].GetCollisionFreeList(shortFirst);
				}
				hasPath = true;
				for (int i = 0; i < ArmSolutions.Length; i++)
				{
					MovePath[i] = shortFirst[i].PopShortest();
					if (MovePath[i] == null)
					{
					  hasPath = false;
					}
				}
			}
		}
		
		void StartArmMotions()
		{
			StopMultiRobotSearch();
            for (int i = 0; i < Arms.Length; i++)
            {
                //Arms[i].StartMove(MovePath[i]);
            }
		}
		
		public void StartMultiRobotSearch()
		{
			foreach (var arm in Arms)
            {
                arm.StartSearch();
            }
			makingPlan = true;
		}
		
		public void StopMultiRobotSearch()
		{
			foreach (var arm in Arms)
            {
                arm.StopSearch();
            }
			makingPlan = false;
		}
		
        void MultiRobotRRT()
        {
            

            while (makingPlan)
            {
                if (newSolution)
                {

                }
            }
        }
    }
}

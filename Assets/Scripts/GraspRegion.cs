using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics.Analysis;

public class GraspRegion : MonoBehaviour {

    public List<Vector3> GoalPoints;

    private double e1;
    private double e2;

    Collider m_ObjectCollider;

    // Use this for initialization
    void Start () {
        //Fetch the GameObject's collider (make sure they have a Collider component)
        m_ObjectCollider = gameObject.GetComponent<Collider>();
        //Output the GameObject's Collider Bound extents
        Debug.Log("extents : " + m_ObjectCollider.bounds.extents);
        Debug.Log("minimum : " + m_ObjectCollider.bounds.min);
        Debug.Log("maximum : " + m_ObjectCollider.bounds.max);
        DefineGoalRegion();
    }

    // Update is called once per frame
    void Update () {
		
	}
    
    Vector3 GetSuperEllipsoidPoint (float a1, float a2, float a3, double eta, double w)
    {
        var x = Convert.ToSingle(a1 * Math.Pow(Math.Cos(eta),e1) * Math.Pow(Math.Cos(w), e2));
        var y = Convert.ToSingle(a2 * Math.Pow(Math.Cos(eta), e1) * Math.Pow(Math.Sin(w), e2));
        var z = Convert.ToSingle(a3 * Math.Pow(Math.Sin(eta), e1));

        return new Vector3() { x = x, y = y, z = z };
    }

    void DefineGoalRegion()
    {
        DefineShapeParameters();

        var aMin = m_ObjectCollider.bounds.min - new Vector3() { x = 0.6f, y = 0.6f, z = 0.6f };
        var aMax = m_ObjectCollider.bounds.max + new Vector3() { x = 0.6f, y = 0.6f, z = 0.6f };
        var aN = (aMax - aMin) / 100;

        double etaN = Math.PI/20;

        double wMin = 0;
        double wMax = 1;
        double wN = (wMax - wMin) / 100;
        PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();

        /*
        for (float i = aMin.x; i <= aMax.x; i += aN.x)
        {
            for (float j = aMin.y; j <= aMax.y; j += aN.y)
            {
                for (float k = aMin.z; k <= aMax.z; k += aN.z)
                {
                    for (double eta = -Math.PI/2; eta <= Math.PI/2; eta += etaN)
                    {
                        for (double w = wMin; w <= wMax; w += wN)
                        {
                            GoalPoints.Add(GetSuperEllipsoidPoint(i, j, k, eta, w));
                        }
                    }
                }
            }
        }
        */
    }
    
    void DefineShapeParameters()
    {
        // Gradient Descent in here to pick e1 and e2
        e1 = 1;
        e2 = 1;
    }
}

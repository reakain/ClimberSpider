using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : MonoBehaviour {

    public FingerJoint[] JointList { get; private set; }

	// Use this for initialization
	void Start () {
        JointList = GetComponentsInChildren<FingerJoint>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

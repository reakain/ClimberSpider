using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Hand : MonoBehaviour
    {
        public Finger[] FingerList { get; private set; }

        // Use this for initialization
        void Start()
        {
            FingerList = GetComponentsInChildren<Finger>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : Subsystem {

    // Max power draw
    [SerializeField][Range(0, 10)] int maxPower;
    // Fuel draw per unit of power per second
    [SerializeField][Range(0,2)] float fuelEfficiency;
    // Light years per unit of power per second
    [SerializeField][Range(0,10)] float powerEfficiency;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

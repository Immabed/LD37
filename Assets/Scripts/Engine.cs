using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : Subsystem {

    // Max power draw
    [SerializeField]
    [Range(0, 10)]
    int maxPower;
    // Base fuel draw per unit of power per second
    [SerializeField]
    [Range(0, 2)]
    float fuelEfficiency;
    // Light years per unit of power per second
    [SerializeField]
    [Range(0, 10)]
    float powerEfficiency;
    // Fractional increase in fuel draw per extra unit of power
    [SerializeField]
    [Range(0, 2)]
    float fuelEfficiencyModifier;

    int currentPower;

    int currentPowerLimit;


    protected override RepairRecipe[] Recipes
    {
        get
        {
            return new RepairRecipe[2] {
                    new RepairRecipe(3, 0, 0), new RepairRecipe(2, 1, 0) };
        }
        set { }
     }



    public int CurrentPower { get { return currentPower; } }

    public float CurrentSpeed { get { return currentPower * powerEfficiency; } }

    public float CurrentFuelDraw { get { return (currentPower + fuelEfficiencyModifier * (currentPower + currentPower ^ 2 - 2) / 2) * fuelEfficiency; } }


    protected override void RepairSystem()
    {
        base.RepairSystem();
    }

    protected override void DamageSystem()
    {
        if (isDamaged)
        {
            // DO Something
        }
        else
        {
            isDamaged = true;
            currentRecipe = Recipes[0];
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

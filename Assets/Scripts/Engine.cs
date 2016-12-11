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



    public int CurrentPower { get { return currentPower; } }

    public float CurrentSpeed { get { return currentPower * powerEfficiency; } }

    public float CurrentFuelDraw { get { return (currentPower + fuelEfficiencyModifier * (currentPower + currentPower ^ 2 - 2) / 2) * fuelEfficiency; } }


    protected override void RepairSystem()
    {
        base.RepairSystem();
        SpriteRenderer sp = gameObject.GetComponentInParent<SpriteRenderer>();
        sp.color = new Color(1, 1, 1);

    }

    public void DoDamage()
    {
        DamageSystem();
    }

    protected override void DamageSystem()
    {
        if (isDamaged)
        {
            // DO Something
        }
        else
        {
            SpriteRenderer sp = gameObject.GetComponentInParent<SpriteRenderer>();
            sp.color = new Color(1, 0, 0);
            isDamaged = true;
            currentRecipe = recipes[0];
            if (currentRecipe.IsCompleted())
            {
                RepairSystem();
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

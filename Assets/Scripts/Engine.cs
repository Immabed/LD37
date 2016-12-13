using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Engine : Subsystem {


    [SerializeField]
    EngineDamage[] damageList;

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

    [SerializeField]
    Text speedTx;
    [SerializeField]
    Text fuelEffTx;
    [SerializeField]
    Text powerUseTx;
    

    int damageLevel = -1;

    public float CurrentSpeed { get { return currentPower * powerEfficiency; } }

    public float CurrentFuelDraw { get { return Mathf.Max((currentPower + fuelEfficiencyModifier * ((currentPower - 1) + (currentPower - 1) ^ 2) / 2) * fuelEfficiency, 0); } }

    protected override IEnumerator UpdateTimer()
    {
        for (;;)
        {
            UpdateUI();
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
    }

    private void UpdateUI() {
        speedTx.text = String.Format("Speed {0:#0.#} light years per second", CurrentSpeed);
        fuelEffTx.text = String.Format("Fuel Efficiency {0:#0.##} fuel per light year", (CurrentFuelDraw / CurrentSpeed));
        powerUseTx.text = String.Format("{0}/{1}", currentPower, maxPower);
        UpdatePowerBars();
    }

    


    private void Awake()
    {

        if (damageList.Length != recipes.Length)
        {
            Debug.LogWarning(String.Format("Engine {0} does not have equal number of repair recipes({1}) and damages({2}). ", gameObject.name, recipes.Length, damageList.Length));
        }
        
    }

    protected override void RepairSystem()
    {
        if (damageLevel >= 0 && currentRecipe.IsCompleted())
        {
            if (damageLevel > 0)
            {
                currentPowerLimit += damageList[damageLevel].PowerDecrease;
                damageLevel--;
                currentRecipe = recipes[damageLevel];
            }
            else
            {
                currentPowerLimit = maxPower;
                damageLevel = -1;
                isDamaged = false;
            }
        }
        UpdatePower();
    }

    public override void DamageSystem()
    {
        
        if (damageLevel < damageList.Length - 1)
        {
            Debug.Log("Damage Level" + damageLevel + ' ' + damageList.Length);
            isDamaged = true;
            damageLevel++;
            currentPowerLimit -= damageList[damageLevel].PowerDecrease;
            currentRecipe = recipes[damageLevel];
            if (currentRecipe.IsCompleted())
            {
                RepairSystem();
            }
            if (damageList[damageLevel].IsFatal && !gm.HasResources(currentRecipe))
            {
                gm.EndGame("Your engines failed in deep space without replacement parts. Maybe a space probe will encounter your frozen remains in future millennia.");
            }
        }
        //Debug.Log("Damage Level" + damageLevel + ' ' + damageList.Length);
        UpdatePower();
    }
}


[System.Serializable]
public struct EngineDamage
{
    [SerializeField]
    int powerDecrease;
    [SerializeField]
    bool isFatal;

    public int PowerDecrease { get { return powerDecrease; } }
    public bool IsFatal { get { return isFatal; } }
}

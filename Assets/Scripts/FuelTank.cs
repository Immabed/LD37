using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelTank : Subsystem {

    [SerializeField]
    FuelDamage[] damageList;
    // Fuel capacity of tank
    [SerializeField]
    int maxFuelCapacity;
    [SerializeField]
    [Tooltip("Each lost power level increases the failure chance X this modifer")]
    float failureRateModifier;
    // Amount of fuel in tanks
    float fuelLevel;
    // Rate of fuel to engines
    float fuelFlow;
    // Rate of fuel leakage
    float fuelLoss;
    // Level of damage
    int damageLevel = -1;
    // Time of last fuel update
    float timeOfLastFuelUpdate;

    [SerializeField]
    Text fuelTx;
    [SerializeField]
    Image fuelLevelSlider;
    [SerializeField]
    Text fuelRateTx;
    [SerializeField]
    Text powerUseTx;
    
    

    public float TotalFuelFlowRate { get { return fuelFlow + fuelLoss; } }
    public float RoomInTank { get { return maxFuelCapacity - fuelLevel; } }
    public float FuelLevel { get { return fuelLevel; } }
    public int FuelCapacity { get { return maxFuelCapacity; } }
    public override float FailureChance
    {
        get
        {
            return base.FailureChance * Mathf.Pow(failureRateModifier, MaxPower - currentPower);
        }
    }


    protected override IEnumerator UpdateTimer()
    {
        for (;;)
        {
            UpdateUI();
            if (fuelLevel < 0)
            {
                fuelLevel = 0;
                OutOfFuel();
                yield break;
            }
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
    } 

    private void Awake()
    {
		type = SubsystemType.FUEL;

        fuelLevel = maxFuelCapacity;
        if (damageList.Length != recipes.Length)
        {
            Debug.LogWarning(String.Format("FuelTank {0} does not have equal number of repair recipes({1}) and damages({2}). ", gameObject.name, recipes.Length, damageList.Length));
        }
    }

    private void OutOfFuel()
    {
        gm.EndGame("How can you run out of fuel when there is fuel available at every station? In space there are no tow trucks, enjoy drifting into the abyss.");
    }


	protected override void UpdateUI()
    {
        fuelFlow = gm.GetFuelUse();
        fuelLevel -= gm.TimeScale * (fuelFlow + fuelLoss) * (Time.time - timeOfLastFuelUpdate);
        timeOfLastFuelUpdate = Time.time;

        fuelLevel = Mathf.Min(fuelLevel, maxFuelCapacity);
        fuelTx.text = String.Format("{0:###}/{1:###}", fuelLevel, maxFuelCapacity);
        fuelRateTx.text = String.Format("{0:#0.00} fuel/sec", TotalFuelFlowRate);
        fuelLevelSlider.fillAmount = fuelLevel / maxFuelCapacity;

        powerUseTx.text = String.Format("{0}/{1}", currentPower, maxPower);
		UpdatePowerBars();
    }

    protected override void RepairSystem()
    {
        //base.RepairSystem();
        if (damageLevel >= 0 && currentRecipe.IsCompleted())
        {
            if (damageLevel > 0)
            {
                fuelLoss -= damageList[damageLevel].FuelLossPerSecond;
                damageLevel--;
                currentRecipe = recipes[damageLevel];
            }
            else
            {
                fuelLoss = 0;
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
            isDamaged = true;
            damageLevel++;
            fuelLoss += damageList[damageLevel].FuelLossPerSecond;
            currentRecipe = recipes[damageLevel];
            if (currentRecipe.IsCompleted())
            {
                RepairSystem();
            }
        }
        UpdatePower();
    }

    public void AddFuel(float amount)
    {
        fuelLevel += amount;
        UpdateUI();
    }
}

[System.Serializable]
public struct FuelDamage
{
    [SerializeField]
    float fuelLossPerSecond;

    public float FuelLossPerSecond { get { return fuelLossPerSecond; } }
}

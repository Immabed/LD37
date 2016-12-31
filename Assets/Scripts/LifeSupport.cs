using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeSupport : Subsystem {

    [SerializeField]
    [Range(0,1)]
    [Tooltip("Fraction of air lost per second when unpowered")]
    float fractionAirLossPerSec;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Fraction of air regained per second under normal operation")]
    float fractionAirGainPerSec;

	[SerializeField]
	Text powerUseTx;
	[SerializeField]
    Slider airLevelIndicator;
    [SerializeField]
    GameObject sliderHandle;
    [SerializeField]
    Text timeTillDeathTx;
    [SerializeField]
    Text percentAirTx;
    [SerializeField]
    Text warningLevelTx;
    [SerializeField]
    Text airStatusTx;
	[SerializeField]
	GameObject repair;
	[SerializeField]
	Text statusTx;
	[SerializeField]
	Text statusDetailsTx;

	// Colors
	[SerializeField]
    Color warningColor;
    [SerializeField]
    Color dangerColor;
    [SerializeField]
    Color goodColor;

    [SerializeField]
    [Range(0,1)]
    [Tooltip("Max fraction of air considered critical")]
    float criticalAirFraction;
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Max fraction of air considered dangerous")]
    float dangerousAirFraction;

    bool isDecreasing;

    [SerializeField]
    float airLevel = 1;

    float timeOfLastUpdate;

    protected override IEnumerator UpdateTimer()
    {
        timeOfLastUpdate = Time.time;
        for (;;)
        {
            if (currentPower != maxPower && airLevel > 0)
            {
                airLevel -= fractionAirLossPerSec * (Time.time - timeOfLastUpdate) * gm.TimeScale * (maxPower - currentPower) / maxPower;
                isDecreasing = true;
               
                if (airLevel <= 0)
                {
                    airLevel = 0;
                    gm.EndGame("Life support failure, classic. The last thing you'll ever deliver is your dead body, how morbid.");
                }
            }
            else if (currentPower == maxPower && airLevel < 1)
            {
                isDecreasing = false;
                airLevel += fractionAirGainPerSec * (Time.time - timeOfLastUpdate) * gm.TimeScale;
            }
            if (airLevel > 1)
            {
                airLevel = 1;
            }
            timeOfLastUpdate = Time.time;
            UpdateUI();
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
    }


    private void Awake()
    {
		type = SubsystemType.LIFESUPPORT;

        if (recipes.Length != 1)
        {
            Debug.LogWarning("Life Support supports having one repair recipe, it currently has" + recipes.Length.ToString());
        }
    }
	protected override void UpdateUI()
    {
		base.UpdateUI();

		// AIR LEVEL SLIDER, SYSTEM STATUS, TIME TILL DEATH, AIR STATUS
        airLevelIndicator.value = airLevel;
		// AIR LEVEL DECREASING
        if (isDecreasing)
        {
			// AIR LEVEL SLIDER HANDLE
            sliderHandle.transform.rotation = Quaternion.Euler(0, 0, 0);

			// TIME TILL DEATH
			int timeTillDeath = (int) (airLevel / (fractionAirLossPerSec * (maxPower - currentPower) / maxPower));
            string minutes = Mathf.Floor(timeTillDeath / 60).ToString("00");
			string seconds = (timeTillDeath % 60).ToString("00");
            timeTillDeathTx.text = String.Format("TIME UNTIL DEATH {0}:{1}", minutes, seconds);

			// AIR STATUS
			airStatusTx.text = "AND FALLING";

			// SYSTEM STATUS
			if (isDamaged) {
				// DAMAGED
				statusTx.text = "SYSTEM DAMAGED";
				statusDetailsTx.text = "AIR LEVEL DECREASING, REPAIR SYSTEM IMMEDIATELY";
				repair.SetActive(true);
			}
			else {
				// UNDERPOWERED
				statusTx.text = "INSUFFICIENT POWER";
				statusDetailsTx.text = string.Format("AIR LEVEL DECREASING, SYSTEM REQUIRES {0} MORE POWER", maxPower - currentPower);
				repair.SetActive(false);
			}
        }
		// SYSTEM NOMINAL
        else
        {
			// AIR LEVEL SLIDER HANDLE
            sliderHandle.transform.rotation = Quaternion.Euler(0, 180, 0);

			// AIR LEVEL INCREASING
            if (airLevel < 1)
            {
				// AIR STATUS
                airStatusTx.text = "AND RISING";
				// TIME TILL FULL
				int timeTillFull = (int) ((1 - airLevel) / (fractionAirGainPerSec));
				string minutes = Mathf.Floor(timeTillFull / 60).ToString("00");
				string seconds = (timeTillFull % 60).ToString("00");
				timeTillDeathTx.text = string.Format("TIME UNTIL FULL {0}:{1}", minutes, seconds);

				//SYSTEM STATUS
				statusTx.text = "SYSTEM NOMINAL, AIR LEVEL LOW";
				statusDetailsTx.text = "AIR LEVEL INCREASING TO BRING AIR PRESSURE TO NOMINAL LEVEL";
            }
			// AIR LEVEL FULL
            else
            {
				// AIR STATUS
                airStatusTx.text = "AND HOLDING";
				// TIME TILL
				timeTillDeathTx.text = "";

				// SYSTEM STATUS
				statusTx.text = "SYSTEM NOMINAL";
				statusDetailsTx.text = "AIR LEVEL NOMINAL";
            }
			repair.SetActive(false);
        }


		// COLOR AND WARNING TEXT
        if (airLevel < criticalAirFraction)
        {
            warningLevelTx.text = "DANGER";
            warningLevelTx.color = dangerColor;
			timeTillDeathTx.color = dangerColor;
        }
        else if (airLevel < dangerousAirFraction)
        {
            warningLevelTx.text = "WARNING";
            warningLevelTx.color = warningColor;
			timeTillDeathTx.color = warningColor;
        }
        else
        {
            warningLevelTx.color = goodColor;
            warningLevelTx.text = "GOOD";
			timeTillDeathTx.color = goodColor;  
        }

		// PERCENT AIR
        percentAirTx.text = String.Format("{0:00}%", 100 * airLevel);

		// POWER USE
		powerUseTx.text = String.Format("{0}/{1}", currentPower, maxPower);
		UpdatePowerBars();
    }


    public override void DamageSystem()
    {
        isDamaged = true;
        currentPowerLimit = 0;
        currentRecipe = recipes[0];
        if (currentRecipe.IsCompleted())
        {
            RepairSystem();
        }
        UpdatePower();
		Debug.Log("Life Support Damaged " + currentPower);
    }

    protected override void RepairSystem()
    {
        base.RepairSystem();
        currentPowerLimit = maxPower;
        UpdatePower();
    }


} 

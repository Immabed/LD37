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
    Text statusTx;
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
        airLevelIndicator.value = airLevel;
        if (isDecreasing)
        {
            sliderHandle.transform.rotation = Quaternion.Euler(0, 0, 0);
            float timeTillDeath = airLevel / (fractionAirLossPerSec * (maxPower - currentPower) / maxPower);
            string minutes = Mathf.Floor(timeTillDeath / 60).ToString("00");
            string seconds = (timeTillDeath % 60).ToString("00");
            timeTillDeathTx.text = String.Format("TIME UNTIL DEATH {0}:{1}", minutes, seconds);
            statusTx.text = "AND FALLING";
        }
        else
        {
            sliderHandle.transform.rotation = Quaternion.Euler(0, 180, 0);
            if (airLevel < 1)
            {
                statusTx.text = "AND RISING";
            }
            else
            {
                statusTx.text = "AND HOLDING";
            }
            timeTillDeathTx.text = "";
        }

        if (airLevel < criticalAirFraction)
        {
            warningLevelTx.text = "DANGER";
            warningLevelTx.color = dangerColor;
        }
        else if (airLevel < dangerousAirFraction)
        {
            warningLevelTx.text = "WARNING";
            warningLevelTx.color = warningColor;
        }
        else
        {
            warningLevelTx.color = goodColor;
            warningLevelTx.text = "GOOD";
            
        }
        percentAirTx.text = String.Format("{0:00}%", 100 * airLevel);
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

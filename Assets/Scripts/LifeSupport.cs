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
    [Range(0,1)]
    [Tooltip("Max fraction of air considered critical")]
    float criticalAirFraction;
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Max fraction of air considered dangerous")]
    float dangerousAirFraction;


    float airLevel = 1;

    float timeOfLastUpdate;

    protected override IEnumerator UpdateTimer()
    {
        timeOfLastUpdate = Time.time;
        for (;;)
        {
            if (currentPower == 0 && airLevel > 0)
            {
                airLevel -= fractionAirLossPerSec * (Time.time - timeOfLastUpdate) * gm.TimeScale;
                if (airLevel <= 0)
                {
                    airLevel = 0;
                    gm.EndGame("Life support failure, classic. The last thing you'll ever deliver is your dead body, how morbid.");
                }
            }
            else if (currentPower > 0 && airLevel < 1)
            {
                airLevel += fractionAirGainPerSec * (Time.time - timeOfLastUpdate) * gm.TimeScale;
            }
            if (airLevel > 1)
            {
                airLevel = 1;
            }
            UpdateUI();
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
    }


    private void Awake()
    {
        if (recipes.Length != 1)
        {
            Debug.LogWarning("Life Support supports having one repair recipe, it currently has" + recipes.Length.ToString());
        }
    }
    private void UpdateUI()
    {
        airLevelIndicator.value = airLevel;
    }

    protected override void DamageSystem()
    {
        isDamaged = true;
        currentPowerLimit = 0;
        currentRecipe = recipes[0];
        if (currentRecipe.IsCompleted())
        {
            RepairSystem();
        }
        UpdatePower();
    }

    protected override void RepairSystem()
    {
        base.RepairSystem();
        currentPowerLimit = maxPower;
        UpdatePower();
    }


} 

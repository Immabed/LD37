using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataBank : Subsystem {

    [SerializeField]
    Text timeTx;
    [SerializeField]
    Text creditCountTx;
    [SerializeField]
    Text cargoDelivered;

    float timeOfLastUpdate;
    float timeTrucking;



    private void UpdateUI()
    {
        int hours = Mathf.FloorToInt(timeTrucking / 3600);
        int minutes = Mathf.FloorToInt((timeTrucking % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeTrucking % 60);
        if (hours > 0)
        {
            timeTx.text = String.Format("Time Trucking {0:#0}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        else
        {
            timeTx.text = String.Format("Time Trucking {0:#0}:{1:00}", minutes, seconds);
        }
        creditCountTx.text = gm.Credits.ToString();
        cargoDelivered.text = String.Format("{0} pieces of cargo delivered", gm.CargoDelivered);
    }

    protected override IEnumerator UpdateTimer()
    {
        for (;;)
        {
            timeTrucking += (Time.time - timeOfLastUpdate) * gm.TimeScale;
            timeOfLastUpdate = Time.time;
            UpdateUI();
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
    }

    protected override void DamageSystem()
    {
        //System can't get damage
        isDamaged = false;
    }
}


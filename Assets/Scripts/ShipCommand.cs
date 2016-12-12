using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipCommand : Subsystem {

    [SerializeField]
    Text timeTx;
    [SerializeField]
    Image progressBar;
    float distanceToDestination;
    float routeLength;
    float timeOfLastUpdate;

    public float DistanceToDestination {  get { return distanceToDestination; } set { routeLength = value; } }

    void UpdateUI()
    {
        timeTx.text = String.Format("Placeholder");
    }

    protected override IEnumerator UpdateTimer()
    {
        timeOfLastUpdate = Time.time;
        for (;;)
        {
            distanceToDestination -= (gm.CurrentSpeed * (Time.time - timeOfLastUpdate) * gm.TimeScale);
            UpdateUI();
            if (distanceToDestination <= 0)
            {
                distanceToDestination = 0;
                gm.ArrivedAtDestination();
                yield break;
            }
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
        
    }

    protected override void DamageSystem()
    {
        isDamaged = false;
        // Does not take damage
    }
}

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
    [SerializeField]
    float routeLength;
    float timeOfLastUpdate;

    public float DistanceToDestination {  get { return distanceToDestination; } set { routeLength = value; } }

    private void Awake()
    {
        distanceToDestination = routeLength;
    }

    void UpdateUI()
    {
        int timeLeft = (int)(distanceToDestination / gm.CurrentSpeed);
        string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
        string seconds = (timeLeft % 60).ToString("00");
        timeTx.text = String.Format("{0} min {1} sec", minutes, seconds);

        progressBar.fillAmount = (routeLength - distanceToDestination) / routeLength;
    }

    protected override IEnumerator UpdateTimer()
    {
        timeOfLastUpdate = Time.time;
        for (;;)
        {
            distanceToDestination -= (gm.CurrentSpeed * (Time.time - timeOfLastUpdate) * gm.TimeScale);
            Debug.Log(distanceToDestination);
            UpdateUI();
            if (distanceToDestination <= 0)
            {
                distanceToDestination = 0;
                gm.ArrivedAtDestination();
                yield break;
            }
            timeOfLastUpdate = Time.time;
            yield return new WaitForSeconds(timeBetweenUpdates);
        }
        
    }

    protected override void DamageSystem()
    {
        isDamaged = false;
        // Does not take damage
    }
}

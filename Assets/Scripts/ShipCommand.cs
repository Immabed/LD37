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

    public float DistanceToDestination {  get { return distanceToDestination; } }


    public void SetNewDestination(float distance)
    {
        routeLength = distance;
        distanceToDestination = distance;
        StartCoroutine(UpdateTimer());
    }

    private void Awake()
    {
		type = SubsystemType.COCKPIT;

        distanceToDestination = routeLength;
    }

    void UpdateUI()
    {
        if (gm.CurrentSpeed != 0)
        {
            int timeLeft = (int)(distanceToDestination / gm.CurrentSpeed);
            string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
            string seconds = (timeLeft % 60).ToString("00");
            timeTx.text = String.Format("{0} min {1} sec", minutes, seconds);
        }
        else
        {
            timeTx.text = String.Format("Stopped");
        }

        progressBar.fillAmount = (routeLength - distanceToDestination) / routeLength;
    }

    protected override IEnumerator UpdateTimer()
    {
        timeOfLastUpdate = Time.time;
        for (;;)
        {
            //Debug.Log(distanceToDestination);

            distanceToDestination -= (gm.CurrentSpeed * (Time.time - timeOfLastUpdate) * gm.TimeScale);
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

    public override void DamageSystem()
    {
        isDamaged = false;
        // Does not take damage
    }
}

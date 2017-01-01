using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerGenerator : Subsystem {


    [SerializeField]
    PowerDamage[] damageList;

    int damageLevel = -1;

    [SerializeField]
    int maxPowerGeneration;
	int currentPowerGeneration;

    [SerializeField]
    Text powerTx;

	[SerializeField]
	PowerBarSelector enginePowerBars;
	[SerializeField]
	PowerBarSelector fuelPowerBars;
	[SerializeField]
	PowerBarSelector lifeSupportPowerBars;


    public int CurrentPowerGeneration { get { return currentPowerGeneration; } }
    public int MaxPowerGeneration { get { return maxPowerGeneration; } }



	protected override void UpdatePowerBars()
	{
		for (int i = 0; i < maxPowerGeneration; i++)
		{
			//Debug.Log("Power Used: " + gm.PowerUsed.ToString());
			if (i < gm.PowerUsed)
			{
				powerBarImages[i].sprite = gm.PowerIcons.InUse;
				powerBars[i].interactable = true;
			}
			else if (i < currentPowerGeneration && i < gm.PowerDrawAvailable)
			{
				powerBarImages[i].sprite = gm.PowerIcons.Available;
				powerBars[i].interactable = true;
			}
			else if (i < currentPowerGeneration)
			{
				powerBarImages[i].sprite = gm.PowerIcons.Unavailable;
				powerBars[i].interactable = false;
			}
			else if (i >= currentPowerGeneration && i < gm.PowerDrawAvailable)
			{
				powerBarImages[i].sprite = gm.PowerIcons.AvailableDisabled;
				powerBars[i].interactable = false;
			}
			else if (i >= currentPowerGeneration)
			{
				powerBarImages[i].sprite = gm.PowerIcons.UnavailableDisabled;
				powerBars[i].interactable = false;
			}
		}
	}

	public void UpdateSystems() {
		enginePowerBars.sys = gm.Engine;
		enginePowerBars.UpdateNumberOfPowerBars();

		fuelPowerBars.sys = gm.FuelTank;
		fuelPowerBars.UpdateNumberOfPowerBars();

		lifeSupportPowerBars.sys = gm.LifeSupport;
		lifeSupportPowerBars.UpdateNumberOfPowerBars();
	}

	public void CopyPowerUsage(PowerGenerator gen)
    {
        // IMPLEMENT
    }

    private void Awake()
    {
		type = SubsystemType.POWER;

        if (damageList.Length != recipes.Length)
        {
            Debug.Log(String.Format("Power Generator {0} does not have equal number of repair recipes({1}) and damages({2}). ", gameObject.name, recipes.Length, damageList.Length));
        }
        currentPowerGeneration = maxPowerGeneration;
    }

    protected override void Start()
    {
		UpdateSystems();
        base.Start();

    }

	protected override void UpdateUI()
    {
		powerTx.text = String.Format("{0}/{1}", gm.PowerUsed, currentPowerGeneration);

		// Systems Power
		enginePowerBars.UpdateUI();
		fuelPowerBars.UpdateUI();
		lifeSupportPowerBars.UpdateUI();
		UpdatePowerBars();
    }

    protected override void RepairSystem()
    {
        //base.RepairSystem();
        if (damageLevel >= 0 && currentRecipe.IsCompleted())
        {
            if (damageLevel > 0)
            {
                currentPowerGeneration += damageList[damageLevel].PowerLoss;
                damageLevel--;
                currentRecipe = recipes[damageLevel];
            }
            else
            {
                currentPowerGeneration = maxPowerGeneration;
                damageLevel = -1;
                isDamaged = false;
            }
        }
        gm.CheckPowerDeficit();
        UpdateUI();
    }

	public override void ClickPower(int id)
	{
		if (id <= currentPowerGeneration && id != gm.PowerUsed)
		{
			int tempPowerLimit = currentPowerGeneration;
			currentPowerGeneration = id;
			gm.IncreasePowerUse();
			gm.CheckPowerDeficit();
			currentPowerGeneration = tempPowerLimit;
			UpdatePowerBars();
		}
		else if (id == gm.PowerUsed)
		{
			int tempPowerLimit = currentPowerGeneration;
			currentPowerGeneration = id - 1;
			gm.IncreasePowerUse();
			gm.CheckPowerDeficit();
			currentPowerGeneration = tempPowerLimit;
			UpdatePowerBars();
		}
		UpdateUI();
	}

    // To satisfy the inheritance. Won't run unless I supply a non-zero update cycle
    protected override IEnumerator UpdateTimer()
    {
		for (;;) {
			UpdateUI();
			yield return new WaitForSeconds(timeBetweenUpdates);
		}
    }

    public override void DamageSystem()
    {
        if (damageLevel < damageList.Length - 1)
        {
            Debug.Log("Damage Level" + damageLevel + ' ' + damageList.Length);
            isDamaged = true;
            damageLevel++;
            currentPowerGeneration -= damageList[damageLevel].PowerLoss;
            currentRecipe = recipes[damageLevel];
            if (currentRecipe.IsCompleted())
            {
                RepairSystem();
            }
            if (damageList[damageLevel].IsFatal && !gm.HasResources(currentRecipe))
            {
                gm.EndGame("You lost power. You can't even call for help, because that would take power. Congrats, idiot.");
            }
        }
        gm.CheckPowerDeficit();
        UpdateUI();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        UpdateUI();
    }
}


[System.Serializable]
public struct PowerDamage
{
    [SerializeField]
    int powerLoss;
    [SerializeField]
    bool isFatal;

    public int PowerLoss { get { return powerLoss; } }
    public bool IsFatal { get { return isFatal; } }
}

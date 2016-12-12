using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoSystem : Subsystem {


    [SerializeField]
    CargoDamage[] damageList;

    int damageLevel = -1;

	[SerializeField]
	int cargoCapacity;
	int cargoUsed;

	Cargo[] cargo;
	int[] powerDistribution;

    [SerializeField]
    SpriteRenderer[] cargoSpriteLocations;



	int PowerNeed {
		get { 
			int powerNeed = 0;
			for (int i = 0; i < cargoUsed; i++) {
				powerNeed += cargo [i].PowerNeed;
				i += cargo [i].Size - 1;
			}
			return powerNeed;
		}
	}

    int CargoRoomAvailable { get {
            UpdateCargoUsed();
            return cargoCapacity - cargoUsed;
        } }

    private void UpdateCargoUsed()
    {
        cargoUsed = 0;
        for (int i = 0; i < cargoCapacity; i++)
        {
            if (cargo[i] != null)
            {
                cargoUsed += cargo[i].Size;
                i += cargo[i].Size - 1;
            }
            else
            {
                break;
            }
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < cargoCapacity; i++)
        {
            if (cargo[i] != null && !cargoSpriteLocations[i].sprite.Equals(cargo[i].Sprite))
            {
                cargoSpriteLocations[i].sprite = cargo[i].Sprite;
            }
            else if (cargo[i] == null && cargoSpriteLocations[i].sprite != null)
            {
                cargoSpriteLocations[i].sprite = null;
            }
        }
    }


    private void Awake()
    {
        // Check for recipes.
        if (damageList.Length != recipes.Length)
        {
            Debug.LogWarning(String.Format("Cargo System {0} does not have equal number of repair recipes({1}) and damages({2}). ", gameObject.name, recipes.Length, damageList.Length));
        }
        cargo = new Cargo[cargoCapacity];
    }

    public void AddCargo(Cargo car)
    {
        if (car != null && CargoRoomAvailable >= car.Size)
        {
            cargo[cargoUsed] = car;
            cargoUsed += car.Size;
            UpdatePower();
            gm.UpdateSystems();
        } 
    }

    public int CollectCargo()
    {
        int creditsEarned = 0;
        for (int i = 0; i < cargoUsed; i++)
        {
            creditsEarned += cargo[i].CurrentCreditValue;
            int step = cargo[i].Size - 1;
            cargo[i] = null;
            i += step;
        }
        UpdateUI();
        return creditsEarned;
    }

    // Need special handling of power due to cargo components needing special care.
    protected override void UpdatePower ()
    {
        base.UpdatePower ();
 
        // Distribute power
        if (currentPower < currentPowerLimit) {
			int powerLeft = currentPowerLimit - currentPower;
            for (int i = 0; i < cargoUsed; i++) {
                if (powerDistribution[i] < cargo[i].PowerNeed)
                {
                    int distributable = Mathf.Min(cargo[i].PowerNeed - powerDistribution[i], powerLeft);
                    powerLeft -= distributable;
                    powerDistribution[i] += distributable;
                    if (powerLeft == 0)
                        break;
                }   
				i += cargo [i].Size - 1;
			}
		}
        for (int i = 0; i < cargoUsed; i++)
        {
            cargo[i].CurrentPower = powerDistribution[i];
            i += cargo[i].Size - 1;
        }

        gm.UpdateSystems();
	}



	protected override IEnumerator UpdateTimer ()
	{
		throw new System.NotImplementedException ();
	}

	protected override void DamageSystem ()
	{
        if (damageLevel < damageList.Length - 1)
        {
            //Debug.Log("Damage Level" + damageLevel + ' ' + damageList.Length);
            isDamaged = true;
            damageLevel++;
            currentPowerLimit = Mathf.RoundToInt((1 - damageList[damageLevel].PowerLossPercent) * PowerNeed);
            currentRecipe = recipes[damageLevel];
            if (currentRecipe.IsCompleted())
            {
                RepairSystem();
            }
        }
        //Debug.Log("Damage Level" + damageLevel + ' ' + damageList.Length);
        UpdatePower();
    }

    protected override void RepairSystem()
    {
        //base.RepairSystem();
        if (damageLevel >= 0 && currentRecipe.IsCompleted())
        {
            if (damageLevel > 0)
            {
                damageLevel--;
                currentPowerLimit = Mathf.RoundToInt((1 - damageList[damageLevel].PowerLossPercent) * PowerNeed);
                currentRecipe = recipes[damageLevel];
            }
            else
            {
                currentPowerLimit = PowerNeed;
                damageLevel = -1;
                isDamaged = false;
            }
        }
        UpdatePower();
    }
}

[System.Serializable]
public struct CargoDamage
{
    [SerializeField]
    [Range(0,1)]
    float powerLossPercent;

    public float PowerLossPercent { get { return powerLossPercent; } }
}





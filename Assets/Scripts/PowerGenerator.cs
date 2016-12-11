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

    public int CurrentPowerGeneration { get { return currentPowerGeneration; } }


    private void Awake()
    {
        if (damageList.Length != recipes.Length)
        {
            Debug.Log(String.Format("Power Generator {0} does not have equal number of repair recipes({1}) and damages({2}). ", gameObject.name, recipes.Length, damageList.Length));
        }
        currentPowerGeneration = maxPowerGeneration;
    }

    protected override void Start()
    {
        base.Start();
        UpdateUI();
    }

    private void UpdateUI()
    {
        powerTx.text = String.Format("Power {0}/{1}", gm.PowerUsed, currentPowerGeneration);
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
        gm.UpdateSystems();
        UpdateUI();
    }

    // To satisfy the inheritance. Won't run unless I supply a non-zero update cycle
    protected override IEnumerator UpdateTimer()
    {
        throw new NotImplementedException();
    }

    protected override void DamageSystem()
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
        gm.UpdateSystems();
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

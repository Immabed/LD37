using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subsystem : MonoBehaviour {

    public GameObject menu;

    // Repair Recipes
    [SerializeField]
    protected abstract RepairRecipe[] Recipes { get; set; }

    protected RepairRecipe currentRecipe;

    protected bool isDamaged;


    protected virtual void Repair(Resource res)
    {
        if (isDamaged) {
            if (res is SpareParts && currentRecipe.SparePartsNeeded > 0)
            {
                currentRecipe.UseSpareParts();
                res.UseResource();
            }
            else if (res is PowerCell && currentRecipe.PowerCellsNeeded > 0)
            {
                currentRecipe.UsePowerCell();
                res.UseResource();
            }
            else if (res is Computer && currentRecipe.ComputersNeeded > 0)
            {
                currentRecipe.UseComputer();
                res.UseResource();
            }
        }
        if (currentRecipe.IsCompleted())
        {
            RepairSystem();
        }
    }

    protected virtual void RepairSystem()
    {
        isDamaged = false;
    }

    protected abstract void DamageSystem();
        
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            menu.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            menu.SetActive(false);
        }
    }
}


public struct RepairRecipe
{
    [SerializeField] int sparePartsNeeded;
    [SerializeField] int computersNeeded;
    [SerializeField] int powerCellsNeeded;

    public int SparePartsNeeded { get { return sparePartsNeeded; } }
    public int ComputersNeeded { get { return computersNeeded; } }
    public int PowerCellsNeeded { get { return powerCellsNeeded; } }

    public RepairRecipe(int spareParts, int computers, int powerCells)
    {
        sparePartsNeeded = spareParts;
        computersNeeded = computers;
        powerCellsNeeded = powerCells;
    }

    public void UseSpareParts()
    {
        if (sparePartsNeeded > 0)
            sparePartsNeeded--;
    }

    public void UseComputer()
    {
        if (computersNeeded > 0)
            computersNeeded--;
    }
    public void UsePowerCell()
    {
        if (powerCellsNeeded > 0)
            powerCellsNeeded--;
    }
    public bool IsCompleted()
    {
        return sparePartsNeeded <= 0 && computersNeeded <= 0 && powerCellsNeeded <= 0;
    }
}

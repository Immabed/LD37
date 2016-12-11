﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subsystem : MonoBehaviour {

    public GameObject menu;
    public GameObject repairMenu;

    // Repair Recipes
    [SerializeField]
    protected RepairRecipe[] recipes;

    protected RepairRecipe currentRecipe;

    protected bool isDamaged;
    protected bool playerIsNearby;
    protected PlayerController pc;

    public bool IsDamaged { get { return isDamaged; } }


    protected virtual void Repair(Resource res)
    {
        if (isDamaged) {
            if (res is SpareParts && currentRecipe.SparePartsNeeded > 0)
            {
                currentRecipe.UseSpareParts();
            }
            else if (res is PowerCell && currentRecipe.PowerCellsNeeded > 0)
            {
                currentRecipe.UsePowerCell();
            }
            else if (res is Computer && currentRecipe.ComputersNeeded > 0)
            {
                currentRecipe.UseComputer();
            }
        }
        if (currentRecipe.IsCompleted())
        {
            RepairSystem();
        }
    }

    // Function for In Game Button to Repair System
    public void TryRepair()
    {
        if (playerIsNearby && pc != null)
        {
            Repair(pc.Item);
            pc.RemoveResource();
        }
    }

    protected virtual void RepairSystem()
    {
        isDamaged = false;
    }

    protected abstract void DamageSystem();

    public bool PlayerCanRepair()
    {
        if (playerIsNearby && pc != null && pc.HasItem && isDamaged && currentRecipe.Needs(pc.Item))
            return true;
        else
            return false;
    }
    public RepairRecipe GetRecipe()
    {
        RepairRecipe recipe;
        if (isDamaged)
        {
            recipe = currentRecipe;
            return recipe;
        }
        else
        {
            return new RepairRecipe(0,0,0);
        }
    }
        
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsNearby = true;
            pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc.HasItem)
            {
                if (isDamaged)
                {
                    repairMenu.SetActive(true);
                }
            }
            else
            {
                menu.SetActive(true);
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsNearby = false;
            pc = null;
            menu.SetActive(false);
            repairMenu.SetActive(false);
        }
    }
}

[System.Serializable]
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
    public bool Needs(Resource res)
    {
        if (res is SpareParts && sparePartsNeeded > 0)
        {
            return true;
        }
        else if (res is Computer && computersNeeded > 0)
        {
            return true;
        }
        else if (res is PowerCell && powerCellsNeeded > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsCompleted()
    {
        return sparePartsNeeded <= 0 && computersNeeded <= 0 && powerCellsNeeded <= 0;
    }
}

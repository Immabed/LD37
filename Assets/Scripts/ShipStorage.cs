using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipStorage : Subsystem {

    [SerializeField]
    int capacity;

    [SerializeField]
    int storedSpareParts;
    int storedPowerCells;
    int storedComputers;

    public int StoredSpareParts { get { return storedSpareParts; } }
    public int StoredPowerCells { get { return storedPowerCells; } }
    public int StoredComputers { get { return storedComputers; } }

    [SerializeField]
    Button getSparePartsBt;
    [SerializeField]
    Button getPowerCellBt;
    [SerializeField]
    Button getComputerBt;
    [SerializeField]
    Button returnResourceBt;

    Text getSparePartsTx;
    Text getPowerCellTx;
    Text getComputerTx;




    protected override IEnumerator UpdateTimer()
    {
        throw new NotImplementedException();
    }

    private void Awake()
    {
        getSparePartsTx = getSparePartsBt.GetComponentInChildren<Text>();
        getComputerTx = getComputerBt.GetComponentInChildren<Text>();
        getPowerCellTx = getPowerCellBt.GetComponentInChildren<Text>();
        UpdateUI();
        isDamaged = true;
    }




    public bool IsFull { get { return !((storedComputers + storedSpareParts + storedPowerCells) < capacity); } }
    public int RoomLeft { get { return capacity - (storedComputers + storedPowerCells + storedSpareParts); } }
    public int MaxCapacity { get { return capacity; } }



    private void UpdateUI()
    {
        getSparePartsTx.text = "Spare Parts " + storedSpareParts.ToString();
        getPowerCellTx.text = "Power Cell " + storedPowerCells.ToString();
        getComputerTx.text = "Computer " + storedComputers.ToString();
        
        getSparePartsBt.interactable = storedSpareParts > 0;
        getComputerBt.interactable = storedComputers > 0;
        getPowerCellBt.interactable = storedPowerCells > 0;

        if (playerIsNearby && pc != null)
        {
            Debug.Log("Player is nearby");
            returnResourceBt.interactable = pc.HasItem;
            if (pc.HasItem)
            {
                getComputerBt.interactable = false;
                getPowerCellBt.interactable = false;
                getSparePartsBt.interactable = false;
            }
        }
    }

    public void AddResource(ResourceType res)
    {
        AddResource(res, 1);
    }

    public void AddResource(ResourceType res, int amount)
    {
        if (amount <= RoomLeft)
        {
            if (res == ResourceType.SPAREPARTS)
                storedSpareParts += amount;
            else if (res == ResourceType.POWERCELL)
                storedPowerCells += amount;
            else if (res == ResourceType.COMPUTER)
                storedComputers += amount;
        }
        UpdateUI();
    }

    public bool TakeResource(ResourceType res)
    {
        if (res == ResourceType.SPAREPARTS && storedSpareParts > 0)
        {
            storedSpareParts--;
            UpdateUI();
            return true;
        }
        else if (res == ResourceType.POWERCELL && storedPowerCells > 0)
        {
            storedPowerCells--;
            UpdateUI();
            return true;
        }
        else if (res == ResourceType.COMPUTER && storedComputers > 0)
        {
            storedComputers--;
            UpdateUI();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasResource(ResourceType res)
    {
        if (res == ResourceType.SPAREPARTS && storedSpareParts > 0)
            return true;
        else if (res == ResourceType.COMPUTER && storedComputers > 0)
            return true;
        else if (res == ResourceType.POWERCELL && storedPowerCells > 0)
            return true;
        else
            return false;
    }

    public void PlayerTakesSpareParts()
    {
        PlayerTakesResource(ResourceType.SPAREPARTS);
    }

    public void PlayerTakesComputer()
    {
        PlayerTakesResource(ResourceType.COMPUTER);
    }

    public void PlayerTakesPowerCell()
    {
        PlayerTakesResource(ResourceType.POWERCELL);
    }

    public void PlayerTakesResource(ResourceType res)
    {
        if (playerIsNearby && pc != null && HasResource(res))
        {
            if (pc.HasItem && pc.Item.Type != res)
            {
                ResourceType inRes = pc.Item.Type;
                pc.RemoveResource();
                pc.SetResource(res);
                TakeResource(res);
                AddResource(inRes);
            }
            if (!pc.HasItem)
            {
                pc.SetResource(res);
                TakeResource(res);
            }
        }
    }

    public void TakePlayersResource()
    {
        //Debug.Log(playerIsNearby.ToString() + ' ' + pc.HasItem.ToString() + ' ' + IsFull.ToString());
        if (playerIsNearby && pc.HasItem && !IsFull)
        {
            //Debug.Log("Player can't return item");
            AddResource(pc.Item.Type);
            pc.RemoveResource();
        }
        UpdateUI();
    }


    public override void DamageSystem()
    {
        // Do nothing, can't be damaged.
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        UpdateUI();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vendor : MonoBehaviour {

	[SerializeField]
	GameManager gm;
    [SerializeField]
    GameObject buttonParent;
    [SerializeField]
    Image shutterImage;

	[SerializeField]
	Button fuelButton;
	[SerializeField]
	Button firstButton;
	[SerializeField]
	Button secondButton;
	[SerializeField]
	Image vendorImage;
	[SerializeField]
	Text vendorName;

	SaleType firstType; // Resource or Upgrade
	SaleType secondType; // Upgrade or Cargo

    FuelSale fuelSale;
	ResourceForSale resource;
	Subsystem upgrade;
	Cargo cargo;

	enum SaleType { UPGRADE, RESOURCE, CARGO}

	public void PickSelection() {
        OpenShutters();

		var vendor = gm.GenerateVendor();
		vendorName.text = vendor.name;
		vendorImage.sprite = vendor.vendor;

		fuelSale = gm.GenerateFuelSale();
		fuelButton.GetComponentInChildren<Text>().text = string.Format("{0} fuel for {1} credits", fuelSale.amount, fuelSale.cost);

		if (Random.value < 0.8) {
			firstType = SaleType.RESOURCE;
			resource = gm.GenerateResourceSale();
			if (Random.value < 0.75) {
				secondType = SaleType.CARGO;
				cargo = gm.GenerateCargoContract();
			}
			else {
				secondType = SaleType.UPGRADE;
				upgrade = gm.GenerateUpgrade();
			}
		}
		else {
			firstType = SaleType.UPGRADE;
			upgrade = gm.GenerateUpgrade();
			secondType = SaleType.CARGO;
			cargo = gm.GenerateCargoContract();
		}
		// UI!!!!!


	}

    // Updates selectability of buttons.
	public void UpdateSelectable() {
        // First Button
        if (firstType == SaleType.RESOURCE)
        {
            firstButton.interactable = (resource.cost <= gm.Credits && resource.amount <= gm.StorageAvailable);
        }
        else if (firstType == SaleType.UPGRADE)
        {
            firstButton.interactable = (upgrade.Cost <= gm.Credits);
        }
        // Second Button
        if (secondType == SaleType.UPGRADE)
        {
            secondButton.interactable = (upgrade.Cost <= gm.Credits);
        }
        else if (secondType == SaleType.CARGO)
        {
            secondButton.interactable = (cargo.Size <= gm.CargoRoomAvailable);
        }

        // Fuel Button
        fuelButton.interactable = (fuelSale.cost <= gm.Credits && fuelSale.amount <= gm.FuelTankRoom);
	}


    public void OpenShutters()
    {
        buttonParent.SetActive(true);
        shutterImage.enabled = false;
    }

	public void CloseShutters() {
        buttonParent.SetActive(false);
        shutterImage.enabled = true;
	}


    public void SelectFirst()
    {
        if (firstType == SaleType.RESOURCE)
        {
            if (gm.BuyResource(resource))
                CloseShutters();
            else
                UpdateSelectable();
        }
        else if (firstType == SaleType.UPGRADE)
        {
            if (gm.BuyUpgrade(upgrade))
                CloseShutters();
            else
            {
                UpdateSelectable();
            }
        }
    }

    public void SelectSecond()
    {
        if (secondType == SaleType.CARGO)
        {
            if (gm.AcceptCargo(cargo))
                CloseShutters();
            else
                UpdateSelectable();
        }
        else if (secondType == SaleType.UPGRADE)
        {
            if (gm.BuyUpgrade(upgrade))
                CloseShutters();
            else
            {
                UpdateSelectable();
            }
        }
    }

    public void BuyFuel()
    {
        if (gm.BuyFuel(fuelSale))
        {
            CloseShutters();
        }
        else
        {
            UpdateSelectable();
        }
    }

    public void Ignore()
    {
        CloseShutters();
    }


}



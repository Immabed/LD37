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
	VendorButton fuelButton;
	[SerializeField]
	VendorButton firstButton;
	[SerializeField]
	VendorButton secondButton;
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
		FillFuelUI(fuelButton, fuelSale);

		if (Random.value < 0.8) {
			firstType = SaleType.RESOURCE;
			resource = gm.GenerateResourceSale();
			FillResourceUI(firstButton, resource);

			if (Random.value < 0.75) {
				secondType = SaleType.CARGO;
				cargo = gm.GenerateCargoContract();
				FillCargoUI(secondButton, cargo);
			}
			else {
				secondType = SaleType.UPGRADE;
				upgrade = gm.GenerateUpgrade();
				FillUpgradeUI(secondButton, upgrade);
			}
		}
		else {
			firstType = SaleType.UPGRADE;
			upgrade = gm.GenerateUpgrade();
			FillUpgradeUI(firstButton, upgrade);
			secondType = SaleType.CARGO;
			cargo = gm.GenerateCargoContract();
			FillCargoUI(secondButton, cargo);
		}

		UpdateSelectable();
	}

	private void FillFuelUI(VendorButton vb, FuelSale fs) 
	{
		vb.title.text = string.Format("{0} FUEL", fs.amount);
		vb.details.text = string.Format("{0} Credits", fs.cost);
		vb.icon.enabled = true;
		vb.icon.sprite = gm.SaleIcons.Fuel;
		vb.cargoIcon.enabled = false;
	}

	private void FillUpgradeUI(VendorButton vb, Subsystem up)
	{
		vb.title.text = string.Format("{0}", up.Name.ToUpper());
		vb.details.text = string.Format("{0} - {1} Credits", up.Description, up.Cost);
		vb.icon.enabled = true;
		switch (up.Type) {
			case SubsystemType.ENGINE:
				vb.icon.sprite = gm.SaleIcons.UpgradeEngine;
				break;
			case SubsystemType.POWER:
				vb.icon.sprite = gm.SaleIcons.UpgradePower;
				break;
			case SubsystemType.FUEL:
				vb.icon.sprite = gm.SaleIcons.UpgradeFuel;
				break;
			case SubsystemType.STORAGE:
				vb.icon.sprite = gm.SaleIcons.UpgradeStorage;
				break;
			case SubsystemType.CARGO:
				vb.icon.sprite = gm.SaleIcons.UpgradeCargo;
				break;
			default:
				vb.icon.enabled = false;
				break;
		}
		vb.cargoIcon.enabled = false;
	}

	private void FillResourceUI(VendorButton vb, ResourceForSale rs)
	{
		vb.title.text = string.Format("{0} {1}", rs.amount, rs.type.ToString());
		vb.details.text = string.Format("{0} Credits", rs.cost);
		vb.icon.enabled = true;
		switch(rs.type) {
			case ResourceType.COMPUTER:
				vb.icon.sprite = gm.SaleIcons.Computer;
				break;
			case ResourceType.SPAREPARTS:
				vb.icon.sprite = gm.SaleIcons.SpareParts;
				break;
			case ResourceType.POWERCELL:
				vb.icon.sprite = gm.SaleIcons.PowerCell;
				break;
			default:
				vb.icon.enabled = false;
				break;
		}
		vb.cargoIcon.enabled = false;
	}

	private void FillCargoUI(VendorButton vb, Cargo cr)
	{
		vb.title.text = string.Format("{0} - {1}", cr.Name.ToUpper(), cr.Type.ToString());
		vb.details.text = string.Format("{0} Credits", cr.MaxCreditValue);
		vb.icon.enabled = true;
		vb.icon.sprite = gm.SaleIcons.Cargo;
		vb.cargoIcon.enabled = true;
		vb.cargoIcon.sprite = cr.Sprite;
	}

    // Updates selectability of buttons.
	public void UpdateSelectable() {
        // First Button
        if (firstType == SaleType.RESOURCE)
        {
            firstButton.button.interactable = (resource.cost <= gm.Credits && resource.amount <= gm.StorageAvailable);
        }
        else if (firstType == SaleType.UPGRADE)
        {
            firstButton.button.interactable = (upgrade.Cost <= gm.Credits);
        }
        // Second Button
        if (secondType == SaleType.UPGRADE)
        {
            secondButton.button.interactable = (upgrade.Cost <= gm.Credits);
        }
        else if (secondType == SaleType.CARGO)
        {
            secondButton.button.interactable = (cargo.Size <= gm.CargoRoomAvailable);
        }

        // Fuel Button
        fuelButton.button.interactable = (fuelSale.cost <= gm.Credits && fuelSale.amount <= gm.FuelTankRoom);
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


	[System.Serializable]
	struct VendorButton {
		public Button button;
		public Text title;
		public Text details;
		public Image icon;
		public Image cargoIcon;
	}
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vendor : MonoBehaviour {

	[SerializeField]
	GameManager gm;

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

	SaleType firstType;
	SaleType secondType;

	ResourceForSale resource;
	Subsystem upgrade;
	Cargo cargo;

	int fuelCost;
	int firstCost;
	int secondCost;

	enum SaleType { UPGRADE, RESOURCE, CARGO}

	public void PickSelection() {
		var vendor = gm.GenerateVendor();
		vendorName.text = vendor.name;
		vendorImage.sprite = vendor.vendor;

		FuelSale fs = gm.GenerateFuelSale();
		fuelButton.GetComponentInChildren<Text>().text = string.Format("{0} fuel for {1} credits", fs.amount, fs.cost);
		fuelCost = fs.cost;

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

	public void UpdateSelectable() {
		// Do stuff
	}

	public void CloseShutters() {
		
	}


}



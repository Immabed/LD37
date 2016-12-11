using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoSystem : Subsystem {

	[SerializeField]
	int cargoCapacity;
	int cargoUsed;

	Cargo[] cargo;
	int[] powerDistribution;

	int damageToPower;

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





	protected override void UpdatePower ()
	{
		currentPowerLimit = PowerNeed - damageToPower;
		base.UpdatePower ();
		if (currentPower < currentPowerLimit) {
			int powerLeft = currentPowerLimit - currentPower;
			for (int i = 0; i < cargoUsed; i++) {
				if (powerDistribution[i] < cargo[i].PowerNeed
				i += cargo [i].Size - 1;
			}
		}
	}



	protected override IEnumerator UpdateTimer ()
	{
		throw new System.NotImplementedException ();
	}

	protected override void DamageSystem ()
	{
		throw new System.NotImplementedException ();
	}
}





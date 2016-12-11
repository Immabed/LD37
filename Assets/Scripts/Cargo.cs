using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo {

	[SerializeField]
	int powerNeed;
	[SerializeField]
	int size;
	[SerializeField]
	bool isTimeSensitive;
	[SerializeField]
	float timeLimit;
	[SerializeField]
	string nameOfCargo;




	public int PowerNeed { get { return powerNeed; } }

	public int Size { get { return size; } }


}

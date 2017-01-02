using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateColor : MonoBehaviour {

	[SerializeField]
	Color lightColor;
	[SerializeField]
	Color darkColor;

	void OnValidate()
	{
		foreach (LightColor color in GetComponentsInChildren<LightColor>())
		{
			color.UpdateColor(lightColor);
		}
		foreach (DarkColor color in GetComponentsInChildren<DarkColor>())
		{
			color.UpdateColor(darkColor);
		}
	}
}

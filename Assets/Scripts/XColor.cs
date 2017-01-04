using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public abstract class XColor : MonoBehaviour
{

	Graphic g;

	public void UpdateColor(Color color)
	{
		if (g == null)
			g = GetComponent<Graphic>();
		g.color = color;
		if (g == null)
			Debug.LogWarning("XColor: No graphic found on " + gameObject.name + " in " + GetComponentInParent<Subsystem>().gameObject.name);
	}
}

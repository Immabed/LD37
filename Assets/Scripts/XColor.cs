using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public abstract class XColor : MonoBehaviour
{

	Graphic g;
	// Use this for initialization
	void Start()
	{
		g = GetComponent<Graphic>();
	}

	public void UpdateColor(Color color)
	{
		g.color = color;
	}
}

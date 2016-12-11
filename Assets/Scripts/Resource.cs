using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

    [SerializeField] PlayerController pc;

    public void MakeVisible()
    {
        pc.SetResource(this);
        gameObject.SetActive(true);
    }

    public void UseResource()
    {
        pc.RemoveResource();
        gameObject.SetActive(false);
    }

}

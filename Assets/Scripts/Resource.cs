using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour {

    //[SerializeField] PlayerController pc;

    public abstract ResourceType Type {get; }

    public void MakeVisible()
    {
        gameObject.SetActive(true);
    }

    public void MakeInvisible()
    {
        gameObject.SetActive(false);
    }

}

public enum ResourceType { NONETYPE, SPAREPARTS, COMPUTER, POWERCELL}

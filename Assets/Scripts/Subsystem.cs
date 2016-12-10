using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subsystem : MonoBehaviour {

    public GameObject menu;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger enter");
        if (collision.tag == "Player")
        {
            menu.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger exit");
        if (collision.tag == "Player")
        {
            menu.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {
    [SerializeField]
    private Animation anim;

    private bool isOpen = false;
    private bool isClose = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isOpen)
        {
            isOpen = true;
            isClose = false;
            anim.Play("openDoor");
        }
    }

    private void OnTriggerStay(Collider other)
    {

    }
    private void OnTriggerExit(Collider other)
    {
        if (!isClose)
        {
            isClose = true;
            isOpen = false;
            anim.Play("closeDoor");
        }
    }
}

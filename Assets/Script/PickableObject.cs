using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickableObject : MonoBehaviour
{
    public float pickUpRange = 5;
    public Transform holdParent;
    private GameObject heldObj;
    TextMeshProUGUI UILight;
    TextMeshProUGUI UIPickup;

    private void Start()
    {
        UILight = GameObject.Find("GUI/LightUpInfo").GetComponent<TextMeshProUGUI>();
        UIPickup = GameObject.Find("GUI/PickUpInfo").GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
        {
            if (hit.transform.gameObject.tag == "SwitchableLight")
            {

                UILight.enabled = true;
                if (Input.GetKeyDown(KeyCode.E))
                { 
                    SwitchLight(hit.transform.gameObject);
                }
            }
            else
            {
                UILight.enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (heldObj == null)
                {
                    PickUpObject(hit.transform.gameObject);
                }
                else
                {
                    DropObject();
                }
            }
            if (hit.transform.gameObject.GetComponent<Rigidbody>())
            {
                UIPickup.enabled = true;
            }
            else
            {
                UIPickup.enabled = false;
            }
        }
        else
        {
            UILight.enabled = false;
            UIPickup.enabled = false;

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (heldObj != null)
                {
                    DropObject();
                }
            }
        }
    }

    void PickUpObject(GameObject pickObj)
    {
        if(pickObj.tag == "Coin")
        {
            Destroy(pickObj);
            //ad coin to inventory
        }
        if (pickObj.GetComponent<Rigidbody>())
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            pickObj.GetComponent<Collider>().enabled = false;

            Rigidbody objRig = pickObj.GetComponent<Rigidbody>();
            objRig.isKinematic = true;
            objRig.useGravity = false;   

            objRig.transform.parent = holdParent;
            objRig.transform.position = holdParent.transform.position;
            objRig.transform.rotation = objRig.inertiaTensorRotation;
            heldObj = pickObj;
        }
    }

    void DropObject()
    {
        //Transform newObjectLocation = (player.transform.forward * distance) + player.transform.position;
        Transform player = GameObject.FindWithTag("Player").transform;
        Transform playerCamera = GameObject.FindWithTag("MainCamera").transform;

        Vector3 fwd = playerCamera.TransformDirection(Vector3.forward);
        Vector3 extents = new Vector3(0.2f, 0.2f, 0.2f);
        float distance = 1;
        RaycastHit hit;
        Rigidbody objRig = heldObj.GetComponent<Rigidbody>();

        if (Physics.BoxCast(playerCamera.transform.position, extents, fwd, out hit, playerCamera.transform.rotation, distance))
        {
            distance = hit.distance;
        }

        heldObj.GetComponent<Collider>().enabled = true;

        objRig.useGravity = true;
        objRig.isKinematic = false;
        objRig.transform.rotation = player.rotation;
        objRig.transform.position = playerCamera.transform.forward * distance + playerCamera.transform.position;
        objRig.transform.Translate(new Vector3(0,0.1f, 0.3f));
        heldObj.transform.parent = null;
        heldObj = null;
    }

    public void SwitchLight(GameObject pickObj)
    {
        Light light = pickObj.GetComponentInChildren<Light>();
        ParticleSystem particules = pickObj.GetComponentInChildren<ParticleSystem>();

        if (light.enabled)
        {
            light.enabled = false;
            particules.Stop();
        }
        else
        {
            light.enabled = true;
            particules.Play();
        }
    }
}

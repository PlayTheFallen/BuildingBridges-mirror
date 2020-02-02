﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class MoveableManager : MonoBehaviour
{
    // [SerializeField]
    // private GameObject player;
    [SerializeField]
    private GameObject slot;
    [SerializeField]
    private GameObject container;
    public GameObject holding;

    [SerializeField]
    private float rotationSnap = 90.0f;

    private Coroutine rotateCoroutine;

    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = this.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // Space - Pickup / Drop
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(holding == null)
            {
                Pickup();
            }
            
            else if (!Physics.Raycast(transform.position, transform.forward, 2f))
            {
                Drop();
            }
            return;
        }

        // Rotation
        // Q - Left
        // E - Right
        if(holding != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateObject(-rotationSnap);
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                RotateObject(rotationSnap);
            }
        }
    }

    void Pickup()
    {
        RaycastHit hit;
        if(Physics.Raycast(gameObject.transform.position, transform.forward, out hit, 2f))
        {
            if (hit.collider.GetComponent<MoveableObject>() != null)
            {
                hit.collider.GetComponent<Rigidbody>().isKinematic = true;
                hit.transform.rotation = hit.collider.GetComponent<MoveableObject>().defaultRotation;

                hit.transform.position = slot.transform.position;
                hit.collider.transform.SetParent(slot.transform);
                holding = hit.collider.gameObject;
            }
        }
    }

    void Drop()
    {
        holding.GetComponent<Rigidbody>().isKinematic = false;
        
        holding.transform.position = transform.position + (transform.forward * 2);
        holding.transform.position = playerMovement.RoundVector(holding.transform.position, playerMovement.snapValue);

        holding.transform.SetParent(container.transform);
        holding = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 5f);
    }

    private void RotateObject(float rotateAmount)
    {
        if (rotateCoroutine == null)
            rotateCoroutine = StartCoroutine(LerpRotateObject(rotateAmount, 5f));
    }

    // Y axis only
    private IEnumerator LerpRotateObject(float rotateAmount, float rotateSpeed)
    {
        float t = 0;
        Vector3 eulerAngles = holding.transform.eulerAngles;
        Vector3 target = new Vector3(
            eulerAngles.x,
            eulerAngles.y + rotateAmount,
            eulerAngles.z
        );

        while (t <= 1)
        {
            t += Time.deltaTime * rotateSpeed;
            holding.transform.eulerAngles = Vector3.Lerp(eulerAngles, target, t);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.1f);
        rotateCoroutine = null;
    }
}

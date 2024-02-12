using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectInteraction : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    public TMP_Text ui_text;
    public float moveSpeed = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }
        if (heldObj != null)
        {
            MoveObject();
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true)
            {
                StopClipping();
                ThrowObject();
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {   
        if (pickUpObj.GetComponent<Rigidbody>())
        {   
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            
            heldObjRb.transform.parent = holdPos.transform;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
            
            
            
        }
    }
    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
    }

    void MoveObject()
    {
        // Calculate the target position for the held object
        Vector3 targetPosition = holdPos.position;

        // Calculate the distance between the held object and the target position
        float distanceToTarget = Vector3.Distance(heldObj.transform.position, targetPosition);

        // If the distance is greater than a small threshold, continue moving towards the target
        if (distanceToTarget > 0.01f)
        {
            // Calculate the movement direction
            Vector3 moveDirection = (targetPosition - heldObj.transform.position).normalized;

            // Calculate the movement amount based on moveSpeed and deltaTime
            float moveAmount = moveSpeed * Time.deltaTime;

            // Limit the movement amount to prevent overshooting
            moveAmount = Mathf.Min(moveAmount, distanceToTarget);

            // Move the held object towards the target position
            heldObj.transform.position += moveDirection * moveAmount;
        }
    }

    void ThrowObject()
    {
        Vector3 currentPosition = heldObj.transform.position;

        // Move the object forward by the specified distance along its forward direction
        heldObj.transform.position = currentPosition + heldObj.transform.forward * 0.5f;
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }

    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);

        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ConceptController : MonoBehaviour
{
    [SerializeReference] GameObject conceptText;
    [SerializeReference] Transform visualElement;
    [SerializeReference] XRGrabInteractable interactable;
    [SerializeReference] AudioSource audAddToChest;

    Rigidbody body;

    bool isInsideChest = false;
    bool isInsertedInchest = false;

    Transform chest;

    private void Start()
    {
        conceptText.SetActive(false);
        body = GetComponent<Rigidbody>();

        XRElementsController.instance.insertedToChest += DisableWhenOtherIsInsertedTochest;

        body.AddForce(new Vector3(1.0f, 1.0f, 1.0f), ForceMode.Impulse);
    }

    void DisableWhenOtherIsInsertedTochest()
    {
        if(!isInsertedInchest)
        {
            this.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        if(XRElementsController.instance != null) 
        {
            XRElementsController.instance.insertedToChest -= DisableWhenOtherIsInsertedTochest;
        }
    }

    public void GrabConcept()
    {
        ActivateText();
    }

    public void ActivateText()
    {
        conceptText.SetActive(true);

        conceptText.transform.LookAt(Camera.main.transform.position, Vector3.up);
        conceptText.transform.Rotate(0, 180, 0);
    }

    public void EndingAction()
    {
        ActivateText();
        interactable.enabled = true;
        body.isKinematic = false;
        isInsideChest = false;
        this.transform.parent = null;
        //body.AddForce(new Vector3(0.1f, 0.1f, 0.1f), ForceMode.Impulse);
    }

    public void ReleasingConcept()
    {
        conceptText.SetActive(false);
        if(isInsideChest)
        {
            insertIntoChest();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerChest")
        {
            isInsideChest = true;
            chest = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "PlayerChest")
        {
            isInsideChest = false;
        }
    }

    void insertIntoChest()
    {
        this.transform.parent = chest;
        visualElement.localScale *= .5f;
        interactable.enabled = false;
        body.velocity = Vector3.zero;
        body.isKinematic = true;
        conceptText.SetActive(false);
        audAddToChest.Play();

        isInsertedInchest = true;

        XRElementsController.instance.ObjectInsertedToChest(this);
    }
}

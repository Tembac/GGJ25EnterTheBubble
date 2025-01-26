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

    Transform chest;

    private void Start()
    {
        conceptText.SetActive(false);
        body = GetComponent<Rigidbody>();

        XRElementsController.instance.insertedToChest += DisableWhenOtherIsInsertedTochest;
    }

    void DisableWhenOtherIsInsertedTochest()
    {
        if(!isInsideChest)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void GrabbingConcept()
    {
        conceptText.SetActive(true);
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
        visualElement.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        interactable.enabled = false;
        body.velocity = Vector3.zero;
        body.isKinematic = true;
        conceptText.SetActive(false);
        audAddToChest.Play();

        XRElementsController.instance.ObjectInsertedToChest();
    }
}

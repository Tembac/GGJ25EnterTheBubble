using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeReference] GameObject txt01GO;
    [SerializeReference] GameObject txt02GO;

    private void Start()
    {
        txt01GO.SetActive(true);
        txt02GO.SetActive(false);

        XRElementsController.instance.insertedToChest += TutoStep01;
    }

    private void OnDestroy()
    {
        XRElementsController.instance.insertedToChest -= TutoStep01;
    }

    void TutoStep01()
    {
        txt01GO.SetActive(false);
        txt02GO.SetActive(true);
    }
}

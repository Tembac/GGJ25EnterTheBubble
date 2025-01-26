using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BubbleController : MonoBehaviour
{
    [SerializeField] string nextScene = "";

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerHead")
        {
            if(nextScene != "")
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}

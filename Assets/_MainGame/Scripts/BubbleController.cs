using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BubbleController : MonoBehaviour
{
    [SerializeField] string nextScene = "";

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerHead")
        {
            if(!nextScene.IsNullOrEmpty())
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}

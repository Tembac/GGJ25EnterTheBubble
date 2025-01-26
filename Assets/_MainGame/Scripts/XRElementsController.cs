using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class XRElementsController : MonoBehaviour
{
    [SerializeReference] ARCameraManager passtrough;
    [SerializeReference] Camera _mainCamera;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += SceneLoad;
    }

    void SceneLoad(Scene _scene, LoadSceneMode _loadMode)
    {
        if(_scene.name != "InitialScene")
        {
            passtrough.enabled = false;
            _mainCamera.clearFlags = CameraClearFlags.Skybox;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

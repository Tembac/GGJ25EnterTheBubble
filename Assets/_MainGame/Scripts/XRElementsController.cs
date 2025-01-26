using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class XRElementsController : MonoBehaviour
{
    //Singleton
    private static XRElementsController _instance;
    public static XRElementsController instance
    {
        get
        {
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<XRElementsController>();
            return _instance;
        }
    }

    [SerializeReference] ARCameraManager passtrough;
    [SerializeReference] Camera _mainCamera;
    [SerializeReference] AudioSource _audioEnterBubble;

    [SerializeReference] BubbleController[] _bubblesArray;

    [SerializeReference] BubbleController _finalBubbleController;

    [SerializeReference] Transform chest;
    [SerializeReference] Joint chestJoint;

    [SerializeField] Vector3 outerCenterPosition;
    [SerializeField] float outerRadius = 10f;
    [SerializeField] float innerRadius = 5f;

    public event Action insertedToChest;

    int lastSpawnedIndex = 0;

    bool noMoreBubbles = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += SceneLoad;
    }

    void SceneLoad(Scene _scene, LoadSceneMode _loadMode)
    {
        _audioEnterBubble.Play();

        if(_scene.name != "InitialScene")
        {
            passtrough.enabled = false;
            _mainCamera.clearFlags = CameraClearFlags.Skybox;
        }

        if(_scene.name != "FinalScene")
        {
            Destroy(chestJoint);
        }
    }

    public void ObjectInsertedToChest()
    {
        SpawnBubbles();
        insertedToChest.Invoke();
    }

    void SpawnBubbles()
    {
        if(noMoreBubbles)
        {
            BubbleController finalBubble = Instantiate(_finalBubbleController);
            finalBubble.transform.position = CalculateRandomPoint();
        }
        else
        {
            for(int i = 0; i < 3; i++)
            {
                if(lastSpawnedIndex < _bubblesArray.Length)
                {
                    BubbleController bubble = Instantiate(_bubblesArray[lastSpawnedIndex]);
                    bubble.transform.position = CalculateRandomPoint();
                    lastSpawnedIndex++;
                }
                else
                {
                    // Break early if we've reached the end of the array
                    noMoreBubbles = true;
                    break;
                }
            }
        }

        // Ensure noMoreBubbles is updated after the loop
        if(lastSpawnedIndex >= _bubblesArray.Length)
        {
            noMoreBubbles = true;
        }
    }

    Vector3 CalculateRandomPoint()
    {
        Vector3 randomPoint = Vector3.zero;

        do
        {
            // Generate a random point inside the unit sphere
            randomPoint = UnityEngine.Random.insideUnitSphere;

            // Scale and translate the point to fit the outer sphere
            randomPoint *= outerRadius;
            randomPoint += outerCenterPosition;
        }
        while(Vector3.Distance(randomPoint, outerCenterPosition) < innerRadius);

        return randomPoint;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the outer sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(outerCenterPosition, outerRadius);

        // Draw the inner sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(outerCenterPosition, innerRadius);
    }

}

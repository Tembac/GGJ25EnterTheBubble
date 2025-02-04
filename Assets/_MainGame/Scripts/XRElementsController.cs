using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using DG.Tweening;

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

    [SerializeReference] AudioSource music01AS;
    [SerializeReference] AudioSource music02AS;
    [SerializeReference] AudioSource music03AS;

    [SerializeField] Vector3 chestFinalPosition;
    [SerializeReference] Transform chest;
    [SerializeReference] Joint chestJoint;

    [SerializeField] Vector3 outerCenterPosition;
    [SerializeField] float outerRadius = 10f;
    [SerializeField] float innerRadius = 5f;

    public event Action insertedToChest;

    int lastSpawnedIndex = 0;

    bool noMoreBubbles = false;

    List<ConceptController> conceptsInChest = new List<ConceptController>();

    private void Awake()
    {
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += SceneLoad;
    }

    void SceneLoad(Scene _scene, LoadSceneMode _loadMode)
    {
        _audioEnterBubble.Play();

        if(_scene.name == "InitialScene"
        || _scene.name == "FinalScene")
        {
            passtrough.enabled = true;
            _mainCamera.clearFlags = CameraClearFlags.Color;

            if(!music01AS.isPlaying)
            {
                music01AS.Stop();
                music02AS.Stop();
                music03AS.Stop();
            }
        }
        else 
        {
            passtrough.enabled = false;
            _mainCamera.clearFlags = CameraClearFlags.Skybox;

            if(!music01AS.isPlaying) { music01AS.Play(); }
            if(!music02AS.isPlaying) { music02AS.Play(); }
            if(!music03AS.isPlaying) { music03AS.Play(); }
        }

        if(_scene.name == "FinalScene")
        {
            Destroy(chestJoint);
            //chest.position = chestFinalPosition;
            chest.GetComponent<Rigidbody>().isKinematic = true;
            chest.DOMove(chestFinalPosition, 5.0f).SetEase(Ease.InExpo)
                .OnComplete(()=> {
                    foreach(ConceptController c in conceptsInChest)
                    {
                        c.EndingAction();
                    }

                    Destroy(chest.gameObject);
                });
  
        }
    }

    public void ObjectInsertedToChest(ConceptController concept)
    {
        SpawnBubbles();
        insertedToChest.Invoke();
        conceptsInChest.Add(concept);
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(outerCenterPosition, innerRadius);

        //final chest position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(chestFinalPosition, 0.1f);
    }

}

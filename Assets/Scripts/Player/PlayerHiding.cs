using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class PlayerHiding : MonoBehaviour
{
    public LayerMask hidingLayer;
    public TMP_Text hidingInteractionText;
    public float hitDistance;

    private PlayerController pController;
    private EnemyAI enemyAI;
    private Transform player;
    private PlayerFootsteps pFootsteps;

    public bool isPlayerHidden;
    private Transform lastPosition;
    private GameObject currentHideSpot = null;

    void Start()
    {
        pController = GetComponent<PlayerController>();
        enemyAI = FindObjectOfType<EnemyAI>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        pFootsteps = GetComponent<PlayerFootsteps>();

        checkVariables();
        isPlayerHidden = false;
    }

    void Update()
    {
        displayHidingInteraction();

        if (Input.GetKeyDown(KeyCode.E))
        {
            engageHiding();
        }
    }

    #region engageHiding
    void engageHiding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (isPlayerHidden) // NOT HIDDEN STATE
        {
            pFootsteps.enabled = true;

            isPlayerHidden = false;
            pController.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //player.transform.position = lastPosition.position;

            changePos(lastPosition, 1);
            Debug.Log("Hrac je momentalne VIDITELNY!");
            // vrati nic, hrac se prestane schovavat
        }

        else if (Physics.Raycast(ray, out hit, hitDistance, hidingLayer)) // HIDDEN STATE
        {
            pFootsteps.enabled = false;

            lastPosition = player.transform;
            Transform clickedObject = hit.collider.transform;

            // Najdeme podobjekt s tagem "HideSpot"
            foreach (Transform child in clickedObject.root.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("HidePoint"))
                {
                    currentHideSpot = child.gameObject;
                    Debug.Log("Uložen HidePoint: " + currentHideSpot.name);
                    break;
                }
            }

            isPlayerHidden = true;
            pController.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;

            //lastPosition = player.transform;
            changePos(currentHideSpot.transform, 0);
            Debug.Log("Hrac je momentalne SCHOVAN!");
        }
    }
    #endregion
    #region displayHidingInteraction
    void displayHidingInteraction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hitDistance, hidingLayer))
        {
            hidingInteractionText.enabled = true;
        }
        else
        {
            hidingInteractionText.enabled = false;
        }
    }
    #endregion
    #region changePos
    void changePos(Transform targetSpot, int a)
    {
        // BECOME HIDDEN
        if(a == 0)
        {
            pController.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotation;

            player.transform.position = targetSpot.position;
            Debug.Log("Players position changed to:" + targetSpot.position);
        }
        if (a == 1) 
        {
            pController.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
    }
    #endregion

    #region checkVariables
    void checkVariables()
    {
        if (hidingLayer == null)
        {
            Debug.Log("No LAYER is selected");
        }

        if (pController == null)
        {
            Debug.Log("No PLAYER CONTROLLER is selected");
        }

        if (hitDistance <= 0f)
        {
            hitDistance = 2f;
        }

        if (hidingInteractionText == null)
        {
            Debug.Log("No TEXT is selected");
        }
        else
        {
            hidingInteractionText.enabled = false;
        }

        if(enemyAI == null)
        {
            Debug.Log("No ENEMY AI is selected");
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public GameObject debugPanel;
    public TMP_Text debugText;

    private bool isDebugVisible = false;
    private bool isNoclipEnabled = false;

    private PlayerController pController;
    private PlayerFootsteps pFootsteps;
    private Rigidbody playerRigidbody;
    private Transform playerTransform;

    public float noclipSpeed = 5f;
    public float mouseSensitivity = 2f;
    private Vector2 rotation = Vector2.zero;

    void Start()
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
        }
        pController = FindObjectOfType<PlayerController>();
        pFootsteps = FindObjectOfType<PlayerFootsteps>();

        playerRigidbody = pController.GetComponent<Rigidbody>();
        playerTransform = pController.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleDebugMenu();
        }

        if (isDebugVisible)
        {
            debugText.text = "Debug Menu:\n" +
                             "F1 - Toggle Noclip";
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleNoclip();
        }

        if (isNoclipEnabled)
        {
            HandleNoclipMovement();
        }
    }

    void ToggleDebugMenu()
    {
        isDebugVisible = !isDebugVisible;
        debugPanel.SetActive(isDebugVisible);
    }

    void ToggleNoclip()
    {
        isNoclipEnabled = !isNoclipEnabled;
        if (pController != null)
        {
            pController.enabled = !isNoclipEnabled;
            pFootsteps.enabled = !isNoclipEnabled;
        }
        if (playerRigidbody != null)
        {
            playerRigidbody.useGravity = !isNoclipEnabled;
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.isKinematic = isNoclipEnabled;
            Debug.Log("NoClip enabled");
        }
        if (!isNoclipEnabled)
        {
            Debug.Log("Reset player position");
            Debug.Log("NoClip disabled");
            playerTransform.rotation = Quaternion.Euler(0, playerTransform.rotation.eulerAngles.y, 0);
        }
    }

    void HandleNoclipMovement()
    {
        float moveSpeed = noclipSpeed * Time.deltaTime;
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += playerTransform.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= playerTransform.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection -= playerTransform.right;
        if (Input.GetKey(KeyCode.D)) moveDirection += playerTransform.right;
        if (Input.GetKey(KeyCode.Space)) moveDirection += playerTransform.up;
        if (Input.GetKey(KeyCode.LeftControl)) moveDirection -= playerTransform.up;

        playerTransform.position += moveDirection * moveSpeed;

        rotation.x += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        playerTransform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0);
    }
}

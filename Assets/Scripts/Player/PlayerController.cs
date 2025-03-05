using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float crouchSpeed = 1.5f; // Pomalejší rychlost při plížení
    public float mouseSensitivity = 2f;
    public float jumpForce = 7f;
    public Transform cameraTransform;
    public LayerMask groundMask;

    private Rigidbody rb;
    private float rotationX = 0f;

    public bool isStealth { get; private set; } // Přidána veřejná proměnná pro detekci plížení

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Zabrání převracení kapsle
        Cursor.lockState = CursorLockMode.Locked; // Skryje a uzamkne kurzor
    }

    void Update()
    {
        // Rychlost podle stisknutých kláves
        float speed = walkSpeed;
        isStealth = false; // Výchozí stav (není v plížení)

        if (Input.GetKey(KeyCode.LeftShift)) speed = sprintSpeed; // Sprint
        if (Input.GetKey(KeyCode.LeftControl)) 
        {
            speed = crouchSpeed; // Plížení
            isStealth = true; // Pokud drží CTRL, je ve stealth režimu
        }

        // Pohyb postavy
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move * speed * Time.deltaTime);

        // Otáčení kamerou
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}

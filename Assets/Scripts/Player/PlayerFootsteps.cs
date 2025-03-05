using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] grassSteps;
    public AudioClip[] tileSteps;
    public AudioClip[] gravelSteps;

    public float walkStepInterval = 0.5f;   // Interval pro chůzi
    public float sprintStepInterval = 0.3f; // Interval pro sprint
    public float crouchStepInterval = 0.8f; // Interval pro plížení

    public float walkVolume = 0.7f;
    public float sprintVolume = 1.0f;
    public float crouchVolume = 0.4f;

    private float stepTimer = 0f;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>(); // Přístup k rychlosti hráče
    }

    void Update()
    {
        if (IsMoving() && IsGrounded())
        {
            stepTimer += Time.deltaTime;

            float stepInterval = GetStepInterval();
            if (stepTimer >= stepInterval)
            {
                PlayFootstepSound();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstepSound()
    {
        string groundTag = GetGroundTag();

        AudioClip[] stepSounds = tileSteps; // Výchozí zvuk (dlaždice)
        if (groundTag == "Grass") stepSounds = grassSteps;
        if (groundTag == "Tile") stepSounds = tileSteps;
        if (groundTag == "Gravel") stepSounds = gravelSteps;

        if (stepSounds.Length > 0)
        {
            audioSource.volume = GetStepVolume(); // Nastavení hlasitosti podle rychlosti
            audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
        }
    }

    string GetGroundTag()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            return LayerMask.LayerToName(hit.collider.gameObject.layer);
        }
        return "Tile"; // Výchozí povrch
    }

    float GetStepInterval()
    {
        if (Input.GetKey(KeyCode.LeftShift)) return sprintStepInterval;
        if (Input.GetKey(KeyCode.LeftControl)) return crouchStepInterval;
        return walkStepInterval;
    }

    float GetStepVolume()
    {
        if (Input.GetKey(KeyCode.LeftShift)) return sprintVolume;
        if (Input.GetKey(KeyCode.LeftControl)) return crouchVolume;
        return walkVolume;
    }

    bool IsMoving()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 7.5f;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Footstep Audio")]
    public AudioSource audioSource;
    public AudioClip tierraClip;
    public AudioClip sueloClip;
    public AudioClip maderaClip;
    public AudioClip piedraClip;
    public float footstepInterval = 0.5f;

    private float footstepTimer;

    Rigidbody rigidbody;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        // Get the rigidbody on this.
        rigidbody = GetComponent<Rigidbody>();
    }

    void ReproducirSonidoPaso()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            TerrenoTipo terreno = hit.collider.GetComponent<TerrenoTipo>();
            if (terreno != null)
            {
                switch (terreno.tipo)
                {
                    case TipoTerreno.Tierra:
                        audioSource.PlayOneShot(tierraClip);
                        break;
                    case TipoTerreno.Suelo:
                        audioSource.PlayOneShot(sueloClip);
                        break;
                    case TipoTerreno.Piedra:
                        audioSource.PlayOneShot(piedraClip);
                        break;
                    case TipoTerreno.Madera:
                        audioSource.PlayOneShot(maderaClip);
                        break;
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
        Debug.Log("Me muevo");

        // FOOTSTEP SOUND LOGIC
        Vector3 horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        if (horizontalVelocity.magnitude > 0.1f)
        {
            footstepTimer -= Time.fixedDeltaTime;
            if (footstepTimer <= 0f)
            {
                ReproducirSonidoPaso();
                footstepTimer = footstepInterval / (IsRunning ? 1.5f : 1f); // Más rápido si corre
            }
        }
        else
        {
            footstepTimer = 0f; // Reinicia si se detiene
        }
    }
}
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float maxSpeed = 35f;        // Maximum forward speed
    public float maxReverseSpeed = 10f; // Maximum reverse speed
    public float rotationSpeed = 10f;   // Rotation speed
    public float acceleration = 10f;    // Acceleration rate in m/s^2
    public float deceleration = 10f;    // Deceleration rate in m/s^2
    public GameObject rigidBodyObject;  // Reference to the object with the Rigidbody
    private Rigidbody rb;
    private float currentSpeed = 0f;

    void Start()
    {
        rb = rigidBodyObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the specified object.");
        }
    }

    void FixedUpdate()
    {
        // Forward/Backward movement with manual acceleration and deceleration
        float moveDirection = -Input.GetAxis("Vertical");  // Invert the direction
        if (moveDirection != 0)
        {
            currentSpeed += moveDirection * acceleration * Time.fixedDeltaTime;
            if (moveDirection > 0)
            {
                currentSpeed = Mathf.Clamp(currentSpeed, -maxReverseSpeed, maxSpeed);
            }
            else
            {
                currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxReverseSpeed);
            }
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed = Mathf.Max(0, currentSpeed - deceleration * Time.fixedDeltaTime);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed = Mathf.Min(0, currentSpeed + deceleration * Time.fixedDeltaTime);
            }
        }

        Vector3 movement = rb.transform.forward * currentSpeed;
        rb.velocity = movement;

        //Debug.Log("Current speed: " + currentSpeed + ", Velocity: " + rb.velocity);

        // Left/Right rotation with continuous turning
        float rotationDirection = Input.GetAxis("Horizontal");
        if (rotationDirection != 0)
        {
            float rotationAmount = rotationDirection * rotationSpeed * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
            //Debug.Log("Applying rotation: " + deltaRotation); // Check rotation applied
        }
    }
}


// CODE GRAVEYARD \\ -- NO ACCELERATION

//using UnityEngine;

//public class ShipMovement : MonoBehaviour
//{
//    public float speed = 10f;           // Movement speed
//    public float rotationSpeed = 10f;   // Rotation speed
//    public GameObject rigidBodyObject;  // Reference to the object with the Rigidbody
//    private Rigidbody rb;

//    void Start()
//    {
//        rb = rigidBodyObject.GetComponent<Rigidbody>();
//        if (rb == null)
//        {
//            Debug.LogError("Rigidbody not found on the specified object.");
//        }
//    }

//    void FixedUpdate()
//    {
//        // Forward/Backward movement
//        float moveDirection = -Input.GetAxis("Vertical");  // Invert the direction
//        if (moveDirection != 0)
//        {
//            Vector3 movement = rb.transform.forward * moveDirection * speed;
//            rb.velocity = movement;
//            Debug.Log("Setting velocity: " + movement); // Check velocity set
//        }
//        else
//        {
//            rb.velocity = Vector3.zero; // Stop movement when no input
//        }

//        // Left/Right rotation with continuous turning
//        float rotationDirection = Input.GetAxis("Horizontal");
//        if (rotationDirection != 0)
//        {
//            float rotationAmount = rotationDirection * rotationSpeed * Time.fixedDeltaTime;
//            Quaternion deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
//            rb.MoveRotation(rb.rotation * deltaRotation);
//            Debug.Log("Applying rotation: " + deltaRotation); // Check rotation applied
//        }
//    }
//}


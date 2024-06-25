using UnityEngine;

public class CIWSGrindset : MonoBehaviour
{
    public Transform cannonBase;  // Reference to the CannonBase
    public Transform cannonRotationPivot;  // Reference to the CannonRotationPivot
    public Transform cannonPivot;  // Reference to the CannonPivot
    public GameObject bulletPrefab;  // Reference to the bullet prefab
    public Transform bulletSpawnPointLeft;  // Reference to the bullet spawn point
    public Transform bulletSpawnPointRight;  // Reference to the bullet spawn point
    public float rotationSpeed = 2f;  // Speed of rotation
    public float detectionRange = 100f; // Range of detection for the raycast
    public float shootingThresholdAngle = 5f; // Threshold angle to decide if the cannon should shoot
    public float bulletSpeed = 20f;  // Speed of the bullet
    private Transform player;  // Reference to the player transform
    [SerializeField]
    private float fireDelay = 1f;
    private float currentTime = 0f;

    void Start()
    {
        // Automatically find the player in the scene
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            AimAtPlayer();
        }
    }

    private void FixedUpdate()
    {
        currentTime++;
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.position - cannonBase.position;

        // Perform a raycast to see if the player is visible
        if (Physics.Raycast(cannonBase.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.transform == player)
            {
                return true;
            }
        }

        return false;
    }

    void AimAtPlayer()
    {
        Vector3 predictedPosition = PredictPlayerPosition();
        Vector3 directionToPlayer = predictedPosition - cannonBase.position;
        directionToPlayer.y = 0; // Ignore vertical difference for base rotation

        // Snap the base rotation to the player
        Quaternion targetBaseRotation = Quaternion.LookRotation(directionToPlayer);
        cannonBase.rotation = targetBaseRotation;

        // Calculate the vertical rotation for the pivot
        Vector3 localDirectionToPlayer = cannonBase.InverseTransformPoint(predictedPosition);
        float targetPitch = Mathf.Atan2(localDirectionToPlayer.y, localDirectionToPlayer.z) * Mathf.Rad2Deg;

        // Invert the pitch if necessary
        targetPitch = -targetPitch;

        // Snap the vertical rotation to the CannonPivot
        Quaternion targetPivotRotation = Quaternion.Euler(targetPitch, 0, 90);  // Set Z axis to 90
        cannonPivot.localRotation = targetPivotRotation;

        // Check if the cannon is aimed at the player
        float angleToPlayer = Vector3.Angle(cannonPivot.forward, directionToPlayer);
        if (angleToPlayer < shootingThresholdAngle)
        {
            if (currentTime >= fireDelay)
            {
                Shoot();
                currentTime = 0f;
            }
        }
    }

    Vector3 PredictPlayerPosition()
    {
        Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;
        float distance = Vector3.Distance(cannonBase.position, player.position);
        float timeToHit = distance / bulletSpeed;
        return player.position + playerVelocity * timeToHit;
    }

    void Shoot()
    {
        // Instantiate the bullet at the bullet spawn point
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPointLeft.position, bulletSpawnPointLeft.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        GameObject bullet2 = Instantiate(bulletPrefab, bulletSpawnPointRight.position, bulletSpawnPointRight.rotation);
        Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();

        // Set the velocity of the bullet
        rb.velocity = bulletSpawnPointLeft.forward * bulletSpeed;
        rb2.velocity = bulletSpawnPointRight.forward * bulletSpeed;
    }
}

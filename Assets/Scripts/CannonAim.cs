using UnityEngine;

public class CannonAiming : MonoBehaviour
{
    public Transform cannonBase;  // Reference to the CannonBase
    public Transform cannonRotationPivot;  // Reference to the CannonRotationPivot
    public Transform cannonPivot;  // Reference to the CannonPivot
    public GameObject bulletPrefab;  // Reference to the bullet prefab
    public Transform bulletSpawnPoint;  // Reference to the bullet spawn point
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
        Vector3 directionToPlayer = player.position - cannonBase.position;
        directionToPlayer.y = 0; // Ignore vertical difference for base rotation

        // Calculate the target rotation for the base
        Quaternion targetBaseRotation = Quaternion.LookRotation(directionToPlayer);
        cannonBase.rotation = Quaternion.RotateTowards(cannonBase.rotation, targetBaseRotation, rotationSpeed * Time.deltaTime);
            
        // Calculate the vertical rotation for the pivot
        Vector3 localDirectionToPlayer = cannonBase.InverseTransformPoint(player.position);
        float targetPitch = Mathf.Atan2(localDirectionToPlayer.y, localDirectionToPlayer.z) * Mathf.Rad2Deg;

        // Invert the pitch if necessary
        targetPitch = -targetPitch;

        // Apply the vertical rotation to the CannonPivot
        Quaternion targetPivotRotation = Quaternion.Euler(targetPitch, 0, 90);  // Set Z axis to 90
        cannonPivot.localRotation = Quaternion.RotateTowards(cannonPivot.localRotation, targetPivotRotation, rotationSpeed * Time.deltaTime);

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

    void Shoot()
    {
        // Instantiate the bullet at the bullet spawn point
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Set the velocity of the bullet
        rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
    }
}

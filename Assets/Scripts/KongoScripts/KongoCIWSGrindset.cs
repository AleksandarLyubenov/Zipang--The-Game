using UnityEngine;

public class CIWSTracker : MonoBehaviour
{
    public Transform CIWSBase;  // Reference to the CIWS base for yaw rotation
    public Transform DomePivot;  // Reference to the DomePivot for pitch rotation
    public Transform Dome;  // Reference to the Dome
    public GameObject bulletPrefab;  // Reference to the bullet prefab
    public Transform bulletSpawnPoint;  // Reference to the bullet spawn point
    public float rotationSpeed = 2f;  // Speed of rotation
    public float detectionRange = 100f; // Range of detection for the raycast
    public float muzzleVelocity = 1000f;  // Speed of the bullet
    public float roundsPerMinute = 3000f; // Firing rate
    public float shootingThresholdAngle = 5f; // Threshold angle to decide if the cannon should shoot
    public ParticleSystem firingEffect;

    private Transform target;  // Reference to the target transform
    private float fireInterval;  // Time between shots
    private float fireTimer = 0f;

    void Start()
    {
        fireInterval = 60f / roundsPerMinute; // Calculate fire interval based on RPM
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        // Acquire the nearest enemy target
        if (target == null || !CanSeeTarget())
        {
            AcquireTarget();
        }

        if (target != null)
        {
            AimAtTarget();
            if (IsTargetWithinFiringAngle() && CanSeeTarget())
            {
                while (fireTimer >= fireInterval)
                {
                    Shoot();
                    fireTimer -= fireInterval; // Decrease fireTimer by fireInterval for continuous firing
                }
            }
        }
    }

    void AcquireTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("AirTarget");
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        if (nearestEnemy != null && shortestDistance <= detectionRange)
        {
            target = nearestEnemy;
        }
        else
        {
            target = null;
        }
    }

    bool CanSeeTarget()
    {
        if (target == null)
        {
            return false;
        }

        RaycastHit hit;
        Vector3 directionToTarget = target.position - bulletSpawnPoint.position;

        // Perform a raycast to see if the target is visible
        if (Physics.Raycast(bulletSpawnPoint.position, directionToTarget, out hit, detectionRange))
        {
            Debug.DrawRay(bulletSpawnPoint.position, directionToTarget * detectionRange, Color.red);  // Draw the raycast for debugging
            if (hit.transform == target)
            {
                return true;
            }
            else if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("AirTarget"))
            {
                return false;
            }
        }

        return false;
    }

    void AimAtTarget()
    {
        Vector3 predictedPosition = PredictInterceptPosition();
        Vector3 directionToTarget = (predictedPosition - CIWSBase.position).normalized;

        // Rotate the base towards the target (yaw)
        Quaternion targetBaseRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z), Vector3.up);
        CIWSBase.rotation = Quaternion.RotateTowards(CIWSBase.rotation, targetBaseRotation, rotationSpeed * Time.deltaTime);

        // Calculate the vertical rotation for the dome pivot (pitch)
        Vector3 localDirectionToTarget = CIWSBase.InverseTransformDirection(directionToTarget);
        float targetPitch = Mathf.Atan2(localDirectionToTarget.y, localDirectionToTarget.z) * Mathf.Rad2Deg;

        // Adjust the dome pivot pitch on its local Z axis
        Quaternion targetPivotRotation = Quaternion.Euler(0, 0, targetPitch);
        DomePivot.localRotation = Quaternion.RotateTowards(DomePivot.localRotation, targetPivotRotation, rotationSpeed * Time.deltaTime);

        Debug.DrawLine(CIWSBase.position, CIWSBase.position + directionToTarget * detectionRange, Color.green); // Draw direction line for debugging
    }

    Vector3 PredictInterceptPosition()
    {
        Vector3 directionToTarget = target.position - bulletSpawnPoint.position;
        Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity;
        float distanceToTarget = directionToTarget.magnitude;
        float timeToIntercept = distanceToTarget / muzzleVelocity;

        // Calculate the predicted position of the target
        return target.position + targetVelocity * timeToIntercept;
    }

    bool IsTargetWithinFiringAngle()
    {
        Vector3 predictedPosition = PredictInterceptPosition();
        Vector3 directionToTarget = (predictedPosition - CIWSBase.position).normalized;
        float angleToTarget = Vector3.Angle(CIWSBase.forward, directionToTarget);
        return angleToTarget < shootingThresholdAngle;
    }

    void Shoot()
    {
        if (CanSeeTarget())  // Double-check to stop shooting if the target is not visible
        {
            Vector3 preciseDirectionToTarget = (PredictInterceptPosition() - bulletSpawnPoint.position).normalized;

            // Instantiate the bullet at the bullet spawn point
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(preciseDirectionToTarget));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            // Ensure the bullet is fired in the precise direction
            rb.velocity = preciseDirectionToTarget * muzzleVelocity;

            Debug.Log("Bullet fired with velocity: " + rb.velocity);

            if (firingEffect != null)
            {
                firingEffect.Play();
            }
        }
    }
}

using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform baseRotation;
    public Transform cannon;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public float rpm = 60.0f; // Rounds per minute
    public float muzzleVelocity = 50.0f; // Bullet speed
    public ParticleSystem firingEffect;
    public float traverseSpeed = 30.0f; // Speed of base rotation in degrees per second
    public float elevationSpeed = 30.0f; // Speed of cannon elevation in degrees per second
    public float targetLeadFactor = 1.0f; // Factor to lead the target

    private float fireInterval;
    private float fireTimer;
    private Transform lockedTarget;

    void Start()
    {
        fireInterval = 60.0f / rpm;
        fireTimer = fireInterval; // Start timer so it can fire immediately
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            LockTarget();
        }

        if (lockedTarget != null)
        {
            RotateTurret();

            if (Input.GetMouseButton(0) && fireTimer >= fireInterval)
            {
                Fire();
                fireTimer = 0.0f; // Reset the timer after firing
            }
        }

        fireTimer += Time.deltaTime; // Update the timer

        // Draw the raycast for debugging purposes
        DrawRaycast();
    }

    void LockTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            lockedTarget = hit.transform;
            Debug.Log("Locked onto target: " + lockedTarget.name);
        }
    }

    void RotateTurret()
    {
        if (lockedTarget == null)
        {
            return;
        }

        Vector3 targetPosition = GetPredictedTargetPosition();

        // Rotate base (Y axis)
        Vector3 baseTargetDir = targetPosition - baseRotation.position;
        baseTargetDir.y = 0; // Keep only the horizontal direction
        Quaternion baseRotationTarget = Quaternion.LookRotation(baseTargetDir);

        baseRotation.rotation = Quaternion.RotateTowards(baseRotation.rotation, baseRotationTarget, traverseSpeed * Time.deltaTime);

        // Rotate cannon (Z axis for elevation)
        Vector3 cannonTargetDir = targetPosition - cannon.position;
        Vector3 cannonLocalTargetDir = baseRotation.InverseTransformDirection(cannonTargetDir);
        float targetElevationAngle = Mathf.Atan2(cannonLocalTargetDir.y, cannonLocalTargetDir.z) * Mathf.Rad2Deg;

        float currentElevationAngle = cannon.localEulerAngles.z;
        currentElevationAngle = (currentElevationAngle > 180) ? currentElevationAngle - 360 : currentElevationAngle;

        float elevationStep = elevationSpeed * Time.deltaTime;
        float newElevationAngle = Mathf.MoveTowards(currentElevationAngle, targetElevationAngle, elevationStep);
        cannon.localEulerAngles = new Vector3(0, 0, newElevationAngle);

        Debug.DrawLine(baseRotation.position, targetPosition, Color.green); // Line to visualize base aiming
        Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward * 10, Color.blue); // Line to visualize bullet spawn forward direction
    }

    Vector3 GetPredictedTargetPosition()
    {
        Vector3 targetPosition = lockedTarget.position;
        Rigidbody targetRigidbody = lockedTarget.GetComponent<Rigidbody>();

        if (targetRigidbody != null)
        {
            Vector3 targetVelocity = targetRigidbody.velocity;
            float distance = Vector3.Distance(bulletSpawn.position, targetPosition);
            float timeToTarget = distance / muzzleVelocity;
            targetPosition += targetVelocity * timeToTarget * targetLeadFactor;
        }

        return targetPosition;
    }

    void Fire()
    {
        // Instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = bulletSpawn.forward * muzzleVelocity;

        // Play firing effect
        if (firingEffect != null)
        {
            firingEffect.Play();
        }
    }

    void DrawRaycast()
    {
        if (lockedTarget != null)
        {
            Debug.DrawLine(baseRotation.position, lockedTarget.position, Color.red); // Red line from baseRotation to target
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        }
    }
}





// CODE GRAVEYARD (I.E. NO TRAVERSE LIMITS)

//using UnityEngine;

//public class TurretController : MonoBehaviour
//{
//    public Transform baseRotation;
//    public Transform cannon;
//    public Transform bulletSpawn;
//    public GameObject bulletPrefab;
//    public float rpm = 60.0f; // Rounds per minute
//    public float muzzleVelocity = 50.0f; // Bullet speed
//    public ParticleSystem firingEffect;
//    public float traverseSpeed = 30.0f; // Speed of base rotation in degrees per second
//    public float elevationSpeed = 30.0f; // Speed of cannon elevation in degrees per second
//    public float targetLeadFactor = 1.0f; // Factor to lead the target

//    private float fireInterval;
//    private float fireTimer;
//    private Transform lockedTarget;

//    void Start()
//    {
//        fireInterval = 60.0f / rpm;
//        fireTimer = 0.0f;
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.T))
//        {
//            LockTarget();
//        }

//        if (lockedTarget != null)
//        {
//            RotateTurret();

//            if (Input.GetMouseButton(0))
//            {
//                Fire();
//            }
//        }

//        // Draw the raycast for debugging purposes
//        DrawRaycast();
//    }

//    void LockTarget()
//    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        if (Physics.Raycast(ray, out RaycastHit hit))
//        {
//            lockedTarget = hit.transform;
//            Debug.Log("Locked onto target: " + lockedTarget.name);
//        }
//    }

//    void RotateTurret()
//    {
//        if (lockedTarget == null)
//        {
//            return;
//        }

//        Vector3 targetPosition = GetPredictedTargetPosition();

//        // Rotate base (Y axis)
//        Vector3 baseTargetDir = targetPosition - baseRotation.position;
//        baseTargetDir.y = 0; // Keep only the horizontal direction
//        Quaternion baseRotationTarget = Quaternion.LookRotation(baseTargetDir);

//        baseRotation.rotation = Quaternion.RotateTowards(baseRotation.rotation, baseRotationTarget, traverseSpeed * Time.deltaTime);

//        // Rotate cannon (Z axis for elevation)
//        Vector3 cannonTargetDir = targetPosition - cannon.position;
//        Vector3 cannonLocalTargetDir = baseRotation.InverseTransformDirection(cannonTargetDir);
//        float targetElevationAngle = Mathf.Atan2(cannonLocalTargetDir.y, cannonLocalTargetDir.z) * Mathf.Rad2Deg;

//        float currentElevationAngle = cannon.localEulerAngles.z;
//        currentElevationAngle = (currentElevationAngle > 180) ? currentElevationAngle - 360 : currentElevationAngle;

//        float elevationStep = elevationSpeed * Time.deltaTime;
//        float newElevationAngle = Mathf.MoveTowards(currentElevationAngle, targetElevationAngle, elevationStep);
//        cannon.localEulerAngles = new Vector3(0, 0, newElevationAngle);

//        Debug.DrawLine(baseRotation.position, targetPosition, Color.green); // Line to visualize base aiming
//        Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward * 10, Color.blue); // Line to visualize bullet spawn forward direction
//    }

//    Vector3 GetPredictedTargetPosition()
//    {
//        Vector3 targetPosition = lockedTarget.position;
//        Rigidbody targetRigidbody = lockedTarget.GetComponent<Rigidbody>();

//        if (targetRigidbody != null)
//        {
//            Vector3 targetVelocity = targetRigidbody.velocity;
//            float distance = Vector3.Distance(bulletSpawn.position, targetPosition);
//            float timeToTarget = distance / muzzleVelocity;
//            targetPosition += targetVelocity * timeToTarget * targetLeadFactor;
//        }

//        return targetPosition;
//    }

//    void Fire()
//    {
//        fireTimer += Time.deltaTime;
//        if (fireTimer >= fireInterval)
//        {
//            fireTimer = 0.0f;

//            // Instantiate bullet
//            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
//            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
//            bulletRb.velocity = bulletSpawn.forward * muzzleVelocity;

//            // Play firing effect
//            if (firingEffect != null)
//            {
//                firingEffect.Play();
//            }
//        }
//    }

//    void DrawRaycast()
//    {
//        if (lockedTarget != null)
//        {
//            Debug.DrawLine(baseRotation.position, lockedTarget.position, Color.red); // Red line from baseRotation to target
//        }
//        else
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
//        }
//    }
//}

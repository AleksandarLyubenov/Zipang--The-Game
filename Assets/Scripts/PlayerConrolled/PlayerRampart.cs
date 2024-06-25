using System.Collections;
using UnityEngine;

public class PlayerRampart : MonoBehaviour
{
    public Transform baseTransform;
    public Transform barrelsRotateTransform;
    public Transform leftBarrelTransform;
    public Transform rightBarrelTransform;
    public GameObject bulletPrefab;
    public float rpm = 60f;
    public float detectionRange = 100f;
    public LayerMask detectionLayer;
    public float bulletForce = 1000f; // Customize the bullet force here
    public float maxElevationAngle = 45f;
    public float maxDepressionAngle = 10f;

    private float fireRate;
    private float nextFireTime;
    private Quaternion initialBaseRotation;
    private Quaternion initialBarrelsRotation;

    void Start()
    {
        if (!AllComponentsAssigned())
        {
            Debug.LogError("Please assign all required transforms and the bullet prefab in the Inspector.");
            enabled = false; // Disable the script if references are missing
            return;
        }

        fireRate = 60f / rpm;
        if (Camera.main == null)
        {
            Debug.LogError("Main camera not found. Ensure there is a camera tagged as 'MainCamera' in the scene.");
            enabled = false; // Disable the script if the camera is not found
            return;
        }

        // Store initial rotations
        initialBaseRotation = baseTransform.localRotation;
        initialBarrelsRotation = barrelsRotateTransform.localRotation;
    }

    void Update()
    {
        RotateTurret();
        if (Input.GetMouseButton(0) && CanFire())
        {
            Fire();
        }
    }

    void RotateTurret()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pointToLook;

        // Check if the ray intersects with a plane
        if (new Plane(Vector3.up, baseTransform.position).Raycast(cameraRay, out float rayLength))
        {
            pointToLook = cameraRay.GetPoint(rayLength);
        }
        else
        {
            pointToLook = cameraRay.GetPoint(1000f); // Arbitrary large distance to simulate infinite distance
        }

        RotateBase(pointToLook);
        RotateBarrels(pointToLook);
    }

    void RotateBase(Vector3 pointToLook)
    {
        Vector3 baseDirection = pointToLook - baseTransform.position;
        baseDirection.y = 0; // Keep the direction horizontal

        if (baseDirection != Vector3.zero)
        {
            Quaternion baseRotation = Quaternion.LookRotation(baseDirection, baseTransform.up);
            baseTransform.localRotation = initialBaseRotation * Quaternion.Euler(0f, baseRotation.eulerAngles.y, 0f); // Adjusted rotation
        }
    }

    void RotateBarrels(Vector3 pointToLook)
    {
        Vector3 barrelsDirection = pointToLook - barrelsRotateTransform.position;
        Quaternion barrelsRotation = Quaternion.LookRotation(barrelsDirection, barrelsRotateTransform.up);

        // Get the angle difference in the local X axis
        float angle = barrelsRotation.eulerAngles.x;
        if (angle > 180) angle -= 360; // Normalize angle to [-180, 180]
        angle = Mathf.Clamp(angle, -maxDepressionAngle, maxElevationAngle);

        barrelsRotateTransform.localRotation = initialBarrelsRotation * Quaternion.Euler(-angle, 0f, 0f);
    }

    bool CanFire()
    {
        RaycastHit hit;
        Vector3 direction = barrelsRotateTransform.forward;

        // Check for obstruction only in front of the barrels
        if (Physics.Raycast(barrelsRotateTransform.position, direction, out hit, detectionRange, detectionLayer))
        {
            return false;
        }
        return Time.time > nextFireTime;
    }

    void Fire()
    {
        if (Time.time > nextFireTime)
        {
            FireBullet(leftBarrelTransform);
            FireBullet(rightBarrelTransform);

            nextFireTime = Time.time + fireRate;
        }
    }

    void FireBullet(Transform barrelTransform)
    {
        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, barrelTransform.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(barrelTransform.forward * bulletForce);
    }

    bool AllComponentsAssigned()
    {
        return baseTransform != null && barrelsRotateTransform != null && leftBarrelTransform != null && rightBarrelTransform != null && bulletPrefab != null;
    }
}



// CODE GRAVEYARD

//using System.Collections;
//using UnityEngine;

//public class PlayerRampart : MonoBehaviour
//{
//    public Transform baseTransform;
//    public Transform barrelsRotateTransform;
//    public Transform leftBarrelTransform;
//    public Transform rightBarrelTransform;
//    public GameObject bulletPrefab;
//    public float rpm = 60f;
//    public float detectionRange = 100f;
//    public LayerMask detectionLayer;
//    public float bulletForce = 1000f; // Customize the bullet force here

//    private float fireRate;
//    private float nextFireTime;

//    private Quaternion initialBaseRotation;
//    private Quaternion initialBarrelsRotation;

//    void Start()
//    {
//        if (baseTransform == null || barrelsRotateTransform == null || leftBarrelTransform == null || rightBarrelTransform == null || bulletPrefab == null)
//        {
//            Debug.LogError("Please assign all required transforms and the bullet prefab in the Inspector.");
//            enabled = false; // Disable the script if references are missing
//            return;
//        }

//        fireRate = 60f / rpm;

//        if (Camera.main == null)
//        {
//            Debug.LogError("Main camera not found. Ensure there is a camera tagged as 'MainCamera' in the scene.");
//            enabled = false; // Disable the script if the camera is not found
//            return;
//        }

//        initialBaseRotation = baseTransform.rotation;
//        initialBarrelsRotation = barrelsRotateTransform.localRotation;
//    }

//    void Update()
//    {
//        RotateTurret();
//        if (Input.GetMouseButton(0) && CanFire())
//        {
//            Fire();
//        }
//    }

//    void RotateTurret()
//    {
//        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
//        Vector3 pointToLook;

//        // Check if the ray intersects with a plane
//        if (new Plane(Vector3.up, baseTransform.position).Raycast(cameraRay, out float rayLength))
//        {
//            pointToLook = cameraRay.GetPoint(rayLength);
//        }
//        else
//        {
//            pointToLook = cameraRay.GetPoint(1000f); // Arbitrary large distance to simulate infinite distance
//        }

//        Vector3 baseDirection = pointToLook - baseTransform.position;
//        baseDirection.y = 0; // Keep the direction horizontal

//        if (baseDirection != Vector3.zero)
//        {
//            Quaternion baseRotation = Quaternion.LookRotation(baseDirection);
//            baseTransform.rotation = initialBaseRotation * Quaternion.Euler(0f, baseRotation.eulerAngles.y, 0f); // Adjusted rotation

//            Vector3 barrelsDirection = pointToLook - barrelsRotateTransform.position;
//            Quaternion barrelsRotation = Quaternion.LookRotation(barrelsDirection);

//            // Adjust barrel rotation to pitch correctly, inverting the pitch
//            barrelsRotateTransform.localRotation = initialBarrelsRotation * Quaternion.Euler(-barrelsRotation.eulerAngles.x, 0f, 0f);
//        }
//    }

//    bool CanFire()
//    {
//        RaycastHit hit;
//        Vector3 direction = barrelsRotateTransform.forward;

//        // Check for obstruction only in front of the barrels
//        if (Physics.Raycast(barrelsRotateTransform.position, direction, out hit, detectionRange, detectionLayer))
//        {
//            return false;
//        }
//        return Time.time > nextFireTime;
//    }

//    void Fire()
//    {
//        if (Time.time > nextFireTime)
//        {
//            // Instantiate bullets with force
//            GameObject bulletLeft = Instantiate(bulletPrefab, leftBarrelTransform.position, leftBarrelTransform.rotation);
//            GameObject bulletRight = Instantiate(bulletPrefab, rightBarrelTransform.position, rightBarrelTransform.rotation);

//            Rigidbody bulletRbLeft = bulletLeft.GetComponent<Rigidbody>();
//            Rigidbody bulletRbRight = bulletRight.GetComponent<Rigidbody>();

//            bulletRbLeft.AddForce(leftBarrelTransform.forward * bulletForce);
//            bulletRbRight.AddForce(rightBarrelTransform.forward * bulletForce);

//            nextFireTime = Time.time + fireRate;
//        }
//    }
//}

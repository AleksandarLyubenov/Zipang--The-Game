using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampartControl : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float rpm = 60.0f;
    public float rotationSpeed = 5.0f;
    public float detectionRange = 1000.0f;
    public LayerMask obstacleLayer;
    public float bulletForce = 1000.0f;

    public Transform baseTransform;
    public Transform barrelsRotateTransform;
    public Transform leftBarrelTransform;
    public Transform rightBarrelTransform;
    private Transform target;
    private float fireRate;
    private float nextFireTime;
    private Rigidbody targetRigidbody;

    void Start()
    {
        fireRate = 60.0f / rpm;
        nextFireTime = Time.time;
    }

    void Update()
    {
        FindTarget();

        if (target != null)
        {
            TrackTarget();
            if (Time.time >= nextFireTime)
            {
                if (CanFire())
                {
                    Fire();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

    void FindTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = detectionRange;
        target = null;
        targetRigidbody = null;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = player.transform;
                targetRigidbody = player.GetComponent<Rigidbody>();
            }
        }
    }

    void TrackTargetOld()
    {
        // Rotate base around local Y axis to face the target
        Vector3 leadPosition = GetLeadPosition();
        Vector3 directionToTarget = leadPosition - baseTransform.position;
        directionToTarget.y = 0; // Ignore height difference for base rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        baseTransform.rotation = Quaternion.RotateTowards(baseTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Rotate BarrelsRotate around local X axis for elevation with pitch limits
        Vector3 localDirection = baseTransform.InverseTransformDirection(leadPosition - barrelsRotateTransform.position);
        float elevationAngle = Mathf.Atan2(localDirection.y, localDirection.z) * Mathf.Rad2Deg;
        elevationAngle = Mathf.Clamp(elevationAngle, -10, 90); // Limit pitch angle

        // Apply the correct elevation rotation
        Quaternion elevationRotation = Quaternion.Euler(elevationAngle, 0, 0);
        barrelsRotateTransform.localRotation = elevationRotation;
    }

    void TrackTarget()
    {
        // Rotate base around local Y axis to face the target
        Vector3 leadPosition = GetLeadPosition();
        Vector3 directionToTarget = leadPosition - baseTransform.position;
        directionToTarget.y = 0; // Ignore height difference for base rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, baseTransform.up);
        baseTransform.rotation = Quaternion.RotateTowards(baseTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Rotate BarrelsRotate around local X axis for elevation with pitch limits
        Vector3 localDirection = baseTransform.InverseTransformDirection(leadPosition - barrelsRotateTransform.position);
        float elevationAngle = Mathf.Atan2(localDirection.y, localDirection.z) * Mathf.Rad2Deg;
        elevationAngle = Mathf.Clamp(elevationAngle, -10, 90); // Limit pitch angle

        // Apply the correct elevation rotation
        Quaternion elevationRotation = Quaternion.Euler(elevationAngle, 0, 0);
        barrelsRotateTransform.localRotation = elevationRotation;
    }


    Vector3 GetLeadPosition()
    {
        if (targetRigidbody == null)
            return target.position;

        Vector3 targetVelocity = targetRigidbody.velocity;
        Vector3 directionToTarget = target.position - leftBarrelTransform.position;
        float distance = directionToTarget.magnitude;
        float bulletTravelTime = distance / bulletForce;

        return target.position + targetVelocity * bulletTravelTime;
    }

    bool CanFire()
    {
        RaycastHit hit;

        Debug.DrawRay(leftBarrelTransform.position, leftBarrelTransform.forward * detectionRange, Color.red);
        if (Physics.Raycast(leftBarrelTransform.position, leftBarrelTransform.forward, out hit, detectionRange, ~obstacleLayer))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        Debug.DrawRay(rightBarrelTransform.position, rightBarrelTransform.forward * detectionRange, Color.red);
        if (Physics.Raycast(rightBarrelTransform.position, rightBarrelTransform.forward, out hit, detectionRange, ~obstacleLayer))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void Fire()
    {
        FireBullet(leftBarrelTransform);
        FireBullet(rightBarrelTransform);
    }

    void FireBullet(Transform barrelTransform)
    {
        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, barrelTransform.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(barrelTransform.forward * bulletForce);
    }
}

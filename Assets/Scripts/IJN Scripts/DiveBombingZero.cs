using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveBombingZero : MonoBehaviour
{
    public float speed = 50f;
    public float turnSpeed = 10f;
    public float liftForce = 5f;
    public float attackRange = 1000f;
    public float fireRate = 1f;
    public float cruiseAltitude = 500f;
    public float diveSpeed = 100f;
    public float pullOutDistance = 200f; // Distance from the target to start pulling out of the dive
    public float diveStartDistance = 200f; // Distance from target to start dive
    public float offsetDistance = 50f; // Offset distance from the target

    public GameObject bulletPrefab;
    public Transform firePoint;

    private Rigidbody rb;
    private GameObject currentTarget;
    private float nextFireTime = 0f;
    private bool isDiving = false;
    private bool isPullingOut = false;
    private bool isCruising = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isCruising)
        {
            DetectTarget();
            CruiseTowardsTarget();
        }
        else if (isDiving)
        {
            DiveTowardsTarget();
            if (Time.time >= nextFireTime)
            {
                FireAtTarget();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        else if (isPullingOut)
        {
            PullOutOfDive();
        }
    }

    void DetectTarget()
    {
        GameObject[] playerShips = GameObject.FindGameObjectsWithTag("PlayerShip");
        float closestDistance = attackRange;
        currentTarget = null;

        foreach (GameObject playerShip in playerShips)
        {
            float distance = Vector3.Distance(transform.position, playerShip.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = playerShip;
            }
        }
    }

    void CruiseTowardsTarget()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.transform.position + Vector3.up * cruiseAltitude + Vector3.forward * offsetDistance;
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = direction * speed;
            transform.rotation = Quaternion.LookRotation(direction); // Point the plane towards the target

            if (Vector3.Distance(transform.position, currentTarget.transform.position) < diveStartDistance)
            {
                isCruising = false;
                isDiving = true;
            }
        }
    }

    void DiveTowardsTarget()
    {
        if (currentTarget != null)
        {
            Vector3 diveDirection = (currentTarget.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(diveDirection); // Point the plane towards the target
            rb.velocity = transform.forward * diveSpeed;

            if (Vector3.Distance(transform.position, currentTarget.transform.position) < pullOutDistance)
            {
                isDiving = false;
                isPullingOut = true;
            }
        }
    }

    void PullOutOfDive()
    {
        Vector3 pullUpDirection = Vector3.up;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pullUpDirection), turnSpeed * Time.fixedDeltaTime);
        rb.velocity = pullUpDirection * speed;

        if (transform.position.y >= cruiseAltitude)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); // Level the plane
            isPullingOut = false;
            isCruising = true;
        }
    }

    void FireAtTarget()
    {
        // Instantiate and fire a bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = firePoint.forward * speed;
    }
}

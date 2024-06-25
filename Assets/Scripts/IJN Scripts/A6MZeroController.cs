using UnityEngine;

public class A6MZeroController : MonoBehaviour
{
    public float cruisingSpeed = 30f;
    public float shootingSpeed = 20f; // 2/3 of cruising speed
    public float retreatDistance = 150f;
    public float retreatDuration = 5f;
    public float bulletForce = 500f;
    public float fireRate = 10f; // Bullets per second
    public GameObject bulletPrefab;
    public Transform gunPoint1;
    public Transform gunPoint2;
    public int maxHealth = 200;

    private int currentHealth;
    private Rigidbody rb;
    private GameObject playerShip;
    private bool isRetreating = false;
    private float retreatTime = 0f;
    private float fireCooldown = 0f;
    private bool isLoitering = false;
    private float loiterTime = 0f;
    private Vector3 retreatDirection;
    private float startingAltitude;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        startingAltitude = transform.position.y;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (playerShip == null)
        {
            Debug.LogWarning("No object with tag 'PlayerShip' found.");
            return;
        }

        Vector3 directionToPlayer = (playerShip.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(playerShip.transform.position, transform.position);

        if (isRetreating)
        {
            Retreat();
        }
        else if (isLoitering)
        {
            Loiter();
        }
        else
        {
            if (distanceToPlayer < retreatDistance)
            {
                StartRetreating();
            }
            else
            {
                ChasePlayer(directionToPlayer, distanceToPlayer);
                ShootAtPlayer(directionToPlayer);
            }
        }

        PointNoseInMovingDirection();

        // Check if health has reached zero
        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Destroy the Zero if health is depleted
        }
    }

    void ChasePlayer(Vector3 direction, float distance)
    {
        float speed = distance < retreatDistance * 2 ? shootingSpeed : cruisingSpeed;
        rb.velocity = direction * speed;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void ShootAtPlayer(Vector3 direction)
    {
        if (fireCooldown <= 0f)
        {
            FireBullet(gunPoint1, direction);
            FireBullet(gunPoint2, direction);
            fireCooldown = 1f / fireRate;
        }
        fireCooldown -= Time.deltaTime;
    }

    void FireBullet(Transform gunPoint, Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = rb.velocity; // Ensure bullet starts with the current velocity of the plane
        bulletRb.AddForce(direction * bulletForce, ForceMode.Impulse);
    }

    void StartRetreating()
    {
        isRetreating = true;
        retreatTime = Time.time;
        retreatDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized; // No altitude change
        rb.velocity = retreatDirection * cruisingSpeed;
    }

    void Retreat()
    {
        if (Time.time - retreatTime > retreatDuration)
        {
            isRetreating = false;
            ReturnToStartingAltitude();
        }
        else
        {
            // Keep nose pointed in the direction of movement
            transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
        }
    }

    void ReturnToStartingAltitude()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, startingAltitude, transform.position.z);
        StartCoroutine(SmoothReturnToAltitude(targetPosition));
    }

    System.Collections.IEnumerator SmoothReturnToAltitude(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 2f; // Time to return to starting altitude
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        DecideNextAction();
    }

    void DecideNextAction()
    {
        if (Random.value < 0.5f) // 50% chance to loiter
        {
            StartLoitering();
        }
        else
        {
            // Resume attacking immediately
        }
    }

    void StartLoitering()
    {
        isLoitering = true;
        loiterTime = Time.time;
    }

    void Loiter()
    {
        if (Time.time - loiterTime > Random.Range(2f, 3f)) // Loiter for 2-3 seconds
        {
            isLoitering = false;
        }
        else
        {
            // Continue flying at the same speed and heading
            rb.velocity = retreatDirection * cruisingSpeed;
            transform.rotation = Quaternion.LookRotation(retreatDirection);
        }
    }

    void PointNoseInMovingDirection()
    {
        if (rb.velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2);
        }
    }
}

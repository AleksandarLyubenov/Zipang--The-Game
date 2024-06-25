using System.Collections;  // Required for IEnumerator
using UnityEngine;

public class BulletDespawn : MonoBehaviour
{
    public int damage = 1; // Default damage for a bullet
    private void OnCollisionEnter(Collision collision)
    {
        // Start the despawn coroutine when the bullet collides with something
        // StartCoroutine(DespawnAfterDelay(3f));

        if (collision.transform.tag == "Player" || collision.transform.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MovePlayer>().GetDamaged();
            Destroy(gameObject);
        }
        else if(collision.transform.CompareTag("AirTarget"))
        {
            AirTarget target = collision.gameObject.GetComponent<AirTarget>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(DespawnAfterDelay(3f));
        }
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Destroy the bullet game object
        Destroy(gameObject);
    }
}

using System.Collections;
using UnityEngine;

public class CannonShellDespawn : MonoBehaviour
{
    public int damage = 100; // Default damage for a cannon shell

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("AirTarget"))
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
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 20;
    public float lifeTime = 3f;

    private Vector2 direction;

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;

        // หัน Sprite ตามทิศทาง
        if (direction.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(
            direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();

            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
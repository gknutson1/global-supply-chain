using Mono.Cecil.Cil;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    GameObject target;
    Vector2 targetPosition;
    bool fired = false;
    bool hits;
    int damage;
    float speed = 5f;

    void Start()
    {
    }

    void Update()
    {
        if (fired)
        {
            Vector3 position = gameObject.transform.position;
            float rotation = gameObject.transform.eulerAngles.z;

            position.y -= Mathf.Sin(rotation * Mathf.Deg2Rad) * speed * Time.deltaTime;
            position.x -= Mathf.Cos(rotation * Mathf.Deg2Rad) * speed * Time.deltaTime;
            gameObject.transform.position = position;

            if (Vector2.Distance(position, targetPosition) <= 0.05f) {
                var ship = target.GetComponent<Ship>();
                if (hits && ship != null) ship.Hit(damage);
                Destroy(gameObject);
            }
        }
    }

    public void Fire(GameObject target, Sprite sprite, bool success, int damage)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;

        this.target = target;
        this.targetPosition = target.transform.position;
        this.hits = success;
        this.damage = damage;

        fired = true;
    }
}

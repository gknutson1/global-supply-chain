using Mono.Cecil.Cil;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    GameObject targetShip;
    Collider2D targetCollider;
    Vector2 targetPosition;
    bool fired = false;
    bool success;
    int damage;
    float speed = 5f;

    void Update()
    {
        if (fired)
        {
            Vector3 position = gameObject.transform.position;
            float rotation = gameObject.transform.eulerAngles.z;

            position.y -= Mathf.Sin(rotation * Mathf.Deg2Rad) * speed * Time.deltaTime;
            position.x -= Mathf.Cos(rotation * Mathf.Deg2Rad) * speed * Time.deltaTime;
            transform.position = position;

            if (Vector2.Distance(position, targetPosition) <= 0.05f)
            {
                if (success)
                {
                    var colliders = Physics2D.OverlapBoxAll(transform.position, Vector2.zero, transform.eulerAngles.z, LayerMask.GetMask("Ships"));
                    var hits = false;
                    foreach (var c in colliders) if (c == targetCollider) hits = true;

                    if (hits && targetShip != null)
                    {
                        var ship = targetShip.GetComponent<Ship>();
                        ship.Hit(damage);
                    }
                }
                Destroy(gameObject);
            }
        }
    }

    public void Fire(Collider2D target, Sprite sprite, bool success, int damage)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;

        targetCollider = target;
        targetShip = targetCollider.gameObject;
        targetPosition = target.transform.position;
        this.success = success;
        this.damage = damage;

        fired = true;
    }
}

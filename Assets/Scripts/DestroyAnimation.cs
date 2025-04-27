using UnityEngine;

public class DestroyAnimation : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    bool animationStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!animationStarted && !spriteRenderer.sprite.name.EndsWith('0')) animationStarted = true;
        else if (animationStarted && spriteRenderer.sprite.name.EndsWith('0')) Destroy(spriteRenderer.gameObject);
    }
}

using UnityEngine;

public class FallingBlockController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public BlockSpawner spawner;
    public HeightManager heightManager;

    public bool IsPlaced => hasFinished;

    private float horizontalInput;
    private Rigidbody rb;
    private bool canControl = true;
    private bool hasFinished = false;
    private bool hasStartedFalling = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    public void SetHorizontalInput(float value)
    {
        if (!canControl) return;

        horizontalInput = value;

        if (!hasStartedFalling && Mathf.Abs(value) > 0.01f)
        {
            StartFalling();
        }
    }

    private void StartFalling()
    {
        hasStartedFalling = true;

        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 velocity = rb.linearVelocity;

        if (canControl)
        {
            velocity.x = horizontalInput * moveSpeed;
        }
        else
        {
            velocity.x = 0f;
        }

        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasFinished) return;

        if (collision.gameObject.CompareTag("BuildSurface") ||
            collision.gameObject.CompareTag("Block"))
        {
            LockControlAndFinish();
        }
    }

    public void NotifyLost()
    {
        if (hasFinished) return;

        hasFinished = true;

        if (spawner != null)
        {
            spawner.ClearCurrentBlock(this);
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.RefreshOffers();
        }
    }

    private void LockControlAndFinish()
    {
        if (hasFinished) return;

        hasFinished = true;
        canControl = false;
        horizontalInput = 0f;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;
        rb.linearVelocity = velocity;

        if (spawner != null)
        {
            spawner.ClearCurrentBlock(this);
        }

        if (heightManager != null)
        {
            heightManager.CheckHeight(GetHighestWorldY());
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.RefreshOffers();
        }
    }

    public float GetHighestWorldY()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        float highestY = transform.position.y;

        foreach (Collider col in colliders)
        {
            if (col.bounds.max.y > highestY)
            {
                highestY = col.bounds.max.y;
            }
        }

        return highestY;
    }

    public Vector3 GetEffectCenter()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        if (colliders == null || colliders.Length == 0)
        {
            return transform.position;
        }

        Bounds bounds = colliders[0].bounds;

        for (int i = 1; i < colliders.Length; i++)
        {
            bounds.Encapsulate(colliders[i].bounds);
        }

        return bounds.center;
    }
}
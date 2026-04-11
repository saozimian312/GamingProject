using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FallingBlockController block = other.GetComponentInParent<FallingBlockController>();

        if (block != null)
        {
            block.NotifyLost();
        }

        Destroy(other.gameObject);
    }
}
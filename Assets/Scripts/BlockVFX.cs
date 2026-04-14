using System.Collections;
using UnityEngine;

public class BlockVFX : MonoBehaviour
{
    public float flashDuration = 0.12f;
    public float lineDuration = 0.08f;

    private Renderer[] renderers;
    private Color[] originalColors;
    private LineRenderer lineRenderer;
    private Coroutine flashRoutine;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.12f;
        lineRenderer.endWidth = 0.04f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    public void FlashAttack()
    {
        StartFlash(Color.red);
    }

    public void FlashIncome()
    {
        StartFlash(Color.yellow);
    }

    public void FlashHeal()
    {
        StartFlash(Color.green);
    }

    public void PlayAttackLine(Vector3 targetPosition)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AttackLineRoutine(targetPosition));
        }
    }

    private void StartFlash(Color color)
    {
        if (!gameObject.activeInHierarchy) return;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            RestoreColors();
        }

        flashRoutine = StartCoroutine(FlashRoutine(color));
    }

    private IEnumerator FlashRoutine(Color color)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                renderers[i].material.color = color;
            }
        }

        yield return new WaitForSeconds(flashDuration);

        RestoreColors();
        flashRoutine = null;
    }

    private IEnumerator AttackLineRoutine(Vector3 targetPosition)
    {
        if (lineRenderer == null) yield break;

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = new Color(1f, 0.3f, 0.3f, 0.2f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPosition);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(lineDuration);

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    private void RestoreColors()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }

    private void OnDisable()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
    }
}
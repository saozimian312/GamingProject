using UnityEngine;

public class HeightManager : MonoBehaviour
{
    public Camera mainCamera;
    public Transform dropPoint;
    public Transform focusTarget;

    [Header("出生点设置")]
    public float spawnOffsetY = 5f;
    public float minDropPointY = 8f;

    [Header("基础相机跟随")]
    public float cameraFollowUpFactor = 0.3f;
    public float cameraFollowBackFactor = 0.4f;
    public float lookTargetYOffset = 2f;

    [Header("额外补偿（防止看不到 DropPoint）")]
    public float extraVisibleMargin = 3f;
    public float extraUpFactor = 0.5f;
    public float extraBackFactor = 0.35f;

    private Vector3 baseCameraPos;
    private Vector3 baseDropPointPos;

    private void Start()
    {
        if (mainCamera != null)
        {
            baseCameraPos = mainCamera.transform.position;
        }

        if (dropPoint != null)
        {
            baseDropPointPos = dropPoint.position;
        }

        ResetForNewLevel();
    }

    public void UpdateForNextSpawn()
    {
        float highestPlacedY = GetHighestPlacedBlockY();

        float targetDropY = baseDropPointPos.y;

        if (highestPlacedY > float.MinValue)
        {
            targetDropY = highestPlacedY + spawnOffsetY;
        }

        targetDropY = Mathf.Max(targetDropY, minDropPointY, baseDropPointPos.y);

        if (dropPoint != null)
        {
            dropPoint.position = new Vector3(
                baseDropPointPos.x,
                targetDropY,
                baseDropPointPos.z
            );
        }

        if (mainCamera != null && focusTarget != null)
        {
            float dropRise = targetDropY - baseDropPointPos.y;

            float baseUp = dropRise * cameraFollowUpFactor;
            float baseBack = dropRise * cameraFollowBackFactor;

            float overHeight = Mathf.Max(
                0f,
                targetDropY - (focusTarget.position.y + extraVisibleMargin)
            );

            float extraUp = overHeight * extraUpFactor;
            float extraBack = overHeight * extraBackFactor;

            Vector3 newCameraPos = baseCameraPos;
            newCameraPos.y += baseUp + extraUp;
            newCameraPos.z -= baseBack + extraBack;

            mainCamera.transform.position = newCameraPos;
            mainCamera.transform.LookAt(focusTarget.position + Vector3.up * lookTargetYOffset);
        }
    }

    public void ResetForNewLevel()
    {
        if (dropPoint != null)
        {
            dropPoint.position = baseDropPointPos;
        }

        if (mainCamera != null)
        {
            mainCamera.transform.position = baseCameraPos;

            if (focusTarget != null)
            {
                mainCamera.transform.LookAt(focusTarget.position + Vector3.up * lookTargetYOffset);
            }
        }
    }

    private float GetHighestPlacedBlockY()
    {
        FallingBlockController[] blocks = FindObjectsByType<FallingBlockController>();
        float highestY = float.MinValue;

        foreach (FallingBlockController block in blocks)
        {
            if (block == null) continue;
            if (!block.IsPlaced) continue;

            float y = block.GetHighestWorldY();
            if (y > highestY)
            {
                highestY = y;
            }
        }

        return highestY;
    }

    // 保留旧接口，防止别的脚本还在调用
    public void CheckHeight(float _)
    {
        UpdateForNextSpawn();
    }
}
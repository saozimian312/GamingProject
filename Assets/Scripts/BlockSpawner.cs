using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public Transform dropPoint;
    public HeightManager heightManager;
    public MobileMoveInput mobileMoveInput;

    private FallingBlockController currentBlock;

    public FallingBlockController SpawnBlock(GameObject prefab)
    {
        if (prefab == null || dropPoint == null) return null;

        GameObject blockObj = Instantiate(prefab, dropPoint.position, Quaternion.identity);

        SetTagRecursively(blockObj.transform, "Block");

        FallingBlockController controller = blockObj.GetComponent<FallingBlockController>();
        if (controller != null)
        {
            controller.spawner = this;
            controller.heightManager = heightManager;

            currentBlock = controller;

            if (mobileMoveInput != null)
            {
                mobileMoveInput.currentBlock = controller;
            }
        }

        return controller;
    }

    private void SetTagRecursively(Transform root, string tagName)
    {
        root.gameObject.tag = tagName;

        foreach (Transform child in root)
        {
            SetTagRecursively(child, tagName);
        }
    }

    public void ClearCurrentBlock(FallingBlockController block)
    {
        if (block == null) return;

        if (currentBlock == block)
        {
            currentBlock = null;
        }

        if (mobileMoveInput != null && mobileMoveInput.currentBlock == block)
        {
            mobileMoveInput.currentBlock = null;
        }
    }

    public bool HasActiveBlock()
    {
        if (currentBlock == null) return false;

        if (currentBlock.IsPlaced)
        {
            ClearCurrentBlock(currentBlock);
            return false;
        }

        return true;
    }
}
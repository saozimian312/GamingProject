using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public Transform dropPoint;
    public MobileMoveInput mobileMoveInput;
    public HeightManager heightManager;

    public bool HasActiveBlock()
    {
        if (mobileMoveInput == null) return false;
        return mobileMoveInput.currentBlock != null;
    }

    public FallingBlockController SpawnBlock(GameObject blockPrefab)
    {
        if (blockPrefab == null)
        {
            Debug.LogError("BlockSpawner: blockPrefab 没有指定。");
            return null;
        }

        if (dropPoint == null)
        {
            Debug.LogError("BlockSpawner: dropPoint 没有指定。");
            return null;
        }

        if (HasActiveBlock())
        {
            Debug.Log("BlockSpawner: 当前已经有活动方块，不能生成新方块。");
            return null;
        }

        GameObject newBlock = Instantiate(blockPrefab, dropPoint.position, Quaternion.identity);

        FallingBlockController controller = newBlock.GetComponent<FallingBlockController>();

        if (controller == null)
        {
            Debug.LogError("BlockSpawner: 生成的 prefab 根物体上没有 FallingBlockController。");
            Destroy(newBlock);
            return null;
        }

        controller.spawner = this;
        controller.heightManager = heightManager;

        if (mobileMoveInput != null)
        {
            mobileMoveInput.currentBlock = controller;
        }
        else
        {
            Debug.LogWarning("BlockSpawner: mobileMoveInput 没有指定，方块会生成，但不能被按钮控制。");
        }

        return controller;
    }

    public void ClearCurrentBlock(FallingBlockController block)
    {
        if (mobileMoveInput == null) return;

        if (mobileMoveInput.currentBlock == block)
        {
            mobileMoveInput.currentBlock = null;
        }
    }
}
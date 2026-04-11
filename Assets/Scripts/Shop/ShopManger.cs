using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public int gold = 100;
    public TMP_Text goldText;

    public BlockSpawner blockSpawner;
    public ShapePrefabEntry[] shapePrefabs;

    public TMP_Text offerText1;
    public TMP_Text offerText2;
    public TMP_Text offerText3;

    private ShopOffer[] currentOffers = new ShopOffer[3];

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGoldUI();
        RefreshOffers();
    }

    public void BuyOffer1()
    {
        BuyOffer(0);
    }

    public void BuyOffer2()
    {
        BuyOffer(1);
    }

    public void BuyOffer3()
    {
        BuyOffer(2);
    }

    private void BuyOffer(int index)
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsGameOver)
{
    return;
}
        Debug.Log("点击购买: " + index);

        if (index < 0 || index >= currentOffers.Length)
        {
            Debug.Log("购买失败：索引错误");
            return;
        }

        if (blockSpawner == null)
        {
            Debug.Log("购买失败：blockSpawner 没绑定");
            return;
        }

        if (blockSpawner.HasActiveBlock())
        {
            Debug.Log("购买失败：当前已经有活动方块");
            return;
        }

        ShopOffer offer = currentOffers[index];

        if (gold < offer.price)
        {
            Debug.Log("购买失败：金币不足");
            return;
        }

        GameObject prefab = GetPrefabByShape(offer.shapeType);
        if (prefab == null)
        {
            Debug.Log("购买失败：没找到对应 prefab -> " + offer.shapeType);
            return;
        }

        if (blockSpawner != null && blockSpawner.heightManager != null)
{
    blockSpawner.heightManager.UpdateForNextSpawn();
}

gold -= offer.price;
UpdateGoldUI();

FallingBlockController block = blockSpawner.SpawnBlock(prefab);
        if (block == null)
        {
            Debug.Log("购买失败：SpawnBlock 返回 null");
            return;
        }

        Debug.Log("已生成方块: " + block.name + " | 形状: " + offer.shapeType + " | 功能: " + offer.blockType);

        ApplyBlockType(block.gameObject, offer.blockType);
    }

    public void RefreshOffers()
    {
        for (int i = 0; i < 3; i++)
        {
            currentOffers[i] = GenerateRandomOffer();
        }

        UpdateOfferUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
    }

    private ShopOffer GenerateRandomOffer()
    {
        ShopOffer offer = new ShopOffer();

        offer.shapeType = (ShapeType)Random.Range(0, System.Enum.GetValues(typeof(ShapeType)).Length);
        offer.blockType = (BlockType)Random.Range(0, System.Enum.GetValues(typeof(BlockType)).Length);

        switch (offer.blockType)
        {
            case BlockType.Attack:
                offer.price = 20;
                break;
            case BlockType.Income:
                offer.price = 15;
                break;
            case BlockType.Heal:
                offer.price = 18;
                break;
        }

        return offer;
    }

    private void UpdateOfferUI()
    {
        if (offerText1 != null) offerText1.text = FormatOffer(currentOffers[0]);
        if (offerText2 != null) offerText2.text = FormatOffer(currentOffers[1]);
        if (offerText3 != null) offerText3.text = FormatOffer(currentOffers[2]);
    }

    private string FormatOffer(ShopOffer offer)
    {
        return offer.shapeType + " + " + offer.blockType + "\n$" + offer.price;
    }

    private GameObject GetPrefabByShape(ShapeType shapeType)
    {
        foreach (ShapePrefabEntry entry in shapePrefabs)
        {
            if (entry.shapeType == shapeType)
            {
                return entry.prefab;
            }
        }

        return null;
    }

    private void ApplyBlockType(GameObject block, BlockType blockType)
    {
        AttackBlock oldAttack = block.GetComponent<AttackBlock>();
        IncomeBlock oldIncome = block.GetComponent<IncomeBlock>();
        HealBlock oldHeal = block.GetComponent<HealBlock>();

        if (oldAttack != null) Destroy(oldAttack);
        if (oldIncome != null) Destroy(oldIncome);
        if (oldHeal != null) Destroy(oldHeal);

        switch (blockType)
        {
            case BlockType.Attack:
                block.AddComponent<AttackBlock>();
                break;

            case BlockType.Income:
                block.AddComponent<IncomeBlock>();
                break;

            case BlockType.Heal:
                block.AddComponent<HealBlock>();
                break;
        }

        Debug.Log("已添加功能: " + blockType);
        Debug.Log("Attack? " + (block.GetComponent<AttackBlock>() != null));
        Debug.Log("Income? " + (block.GetComponent<IncomeBlock>() != null));
        Debug.Log("Heal? " + (block.GetComponent<HealBlock>() != null));
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold;
        }
    }
}
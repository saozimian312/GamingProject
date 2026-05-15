using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Gold")]
    public int gold = 100;
    public TMP_Text goldText;

    [Header("Spawner")]
    public BlockSpawner blockSpawner;
    public ShapePrefabEntry[] shapePrefabs;
    public ShapeSpriteEntry[] shapeSprites;

    [Header("Offer Texts")]
    public TMP_Text offerText1;
    public TMP_Text offerText2;
    public TMP_Text offerText3;

    [Header("Offer Images")]
    public Image offerImage1;
    public Image offerImage2;
    public Image offerImage3;

    [Header("Offer Buttons")]
    public Button offerButton1;
    public Button offerButton2;
    public Button offerButton3;

    [Header("Materials")]
    public Material attackMaterial;
    public Material incomeMaterial;
    public Material healMaterial;

    [Header("Attack Block Stats")]
    public int attackPrice = 20;
    public int attackDamage = 1;
    public float attackInterval = 1f;
    public float attackRange = 2.5f;

    [Header("Income Block Stats")]
    public int incomePrice = 15;
    public int incomeGoldAmount = 5;
    public float incomeInterval = 2f;

    [Header("Heal Block Stats")]
    public int healPrice = 18;
    public int healAmount = 1;
    public float healInterval = 2f;

    private ShopOffer[] currentOffers = new ShopOffer[3];
    private int pendingRefreshIndex = -1;
    private bool purchaseUnlocked = true;
    private FallingBlockController trackedBlock;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGoldUI();
        GenerateAllOffers();
        UpdateOfferUI();
        UpdateButtonStates();
    }

    private void Update()
    {
        CheckTrackedBlockPlacement();
        UpdateButtonStates();
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
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsGameOver) return;
        if (index < 0 || index >= currentOffers.Length) return;
        if (blockSpawner == null) return;
        if (!purchaseUnlocked) return;

        ShopOffer offer = currentOffers[index];
        if (offer == null) return;
        if (gold < offer.price) return;

        if (blockSpawner.heightManager != null)
        {
            blockSpawner.heightManager.UpdateForNextSpawn();
        }

        GameObject prefab = GetPrefabByShape(offer.shapeType);
        if (prefab == null) return;

        gold -= offer.price;
        UpdateGoldUI();

        FallingBlockController block = blockSpawner.SpawnBlock(prefab);
        if (block == null) return;

        trackedBlock = block;
        purchaseUnlocked = false;

        if (blockSpawner.mobileMoveInput != null)
        {
            blockSpawner.mobileMoveInput.currentBlock = block;
        }

        ApplyBlockType(block.gameObject, offer.blockType);

        pendingRefreshIndex = index;
        UpdateButtonStates();
    }

    private void CheckTrackedBlockPlacement()
    {
        if (purchaseUnlocked) return;

        if (trackedBlock == null)
        {
            ForceUnlockPlacement();
            return;
        }

        if (trackedBlock.IsPlaced)
        {
            OnCurrentBlockFinished();
            return;
        }

        if (IsTouchingPlacementSurface(trackedBlock.gameObject))
        {
            OnCurrentBlockFinished();
        }
    }

    private bool IsTouchingPlacementSurface(GameObject blockObj)
    {
        Collider[] colliders = blockObj.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            Collider[] overlaps = Physics.OverlapBox(
                col.bounds.center,
                col.bounds.extents * 0.98f,
                col.transform.rotation
            );

            foreach (Collider hit in overlaps)
            {
                if (hit == null) continue;
                if (hit.transform.IsChildOf(blockObj.transform)) continue;

                if (hit.CompareTag("BuildSurface") || hit.CompareTag("Block"))
                {
                    return true;
                }

                Transform parent = hit.transform.parent;
                while (parent != null)
                {
                    if (parent.CompareTag("BuildSurface") || parent.CompareTag("Block"))
                    {
                        return true;
                    }

                    parent = parent.parent;
                }
            }
        }

        return false;
    }

    private void ForceUnlockPlacement()
    {
        purchaseUnlocked = true;

        if (blockSpawner != null && trackedBlock != null)
        {
            blockSpawner.ClearCurrentBlock(trackedBlock);
        }

        if (blockSpawner != null && blockSpawner.mobileMoveInput != null)
        {
            blockSpawner.mobileMoveInput.currentBlock = null;
        }

        trackedBlock = null;
        pendingRefreshIndex = -1;

        UpdateButtonStates();
    }

    public void OnCurrentBlockFinished()
    {
        if (blockSpawner != null && trackedBlock != null)
        {
            blockSpawner.ClearCurrentBlock(trackedBlock);
        }

        if (blockSpawner != null && blockSpawner.mobileMoveInput != null)
        {
            blockSpawner.mobileMoveInput.currentBlock = null;
        }

        purchaseUnlocked = true;
        trackedBlock = null;

        if (pendingRefreshIndex >= 0 && pendingRefreshIndex < currentOffers.Length)
        {
            currentOffers[pendingRefreshIndex] = GenerateRandomOffer();
            pendingRefreshIndex = -1;
        }

        UpdateOfferUI();
        UpdateButtonStates();
    }

    public void RefreshOffers()
    {
        if (blockSpawner != null && trackedBlock != null)
        {
            blockSpawner.ClearCurrentBlock(trackedBlock);
        }

        if (blockSpawner != null && blockSpawner.mobileMoveInput != null)
        {
            blockSpawner.mobileMoveInput.currentBlock = null;
        }

        purchaseUnlocked = true;
        trackedBlock = null;
        pendingRefreshIndex = -1;

        GenerateAllOffers();
        UpdateOfferUI();
        UpdateButtonStates();
    }

    public void ResetGold(int amount = 100)
    {
        gold = amount;
        UpdateGoldUI();
        UpdateButtonStates();
    }

    private void GenerateAllOffers()
    {
        for (int i = 0; i < 3; i++)
        {
            currentOffers[i] = GenerateRandomOffer();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
        UpdateButtonStates();
    }

    private ShopOffer GenerateRandomOffer()
    {
        ShopOffer offer = new ShopOffer();

        offer.shapeType = (ShapeType)Random.Range(0, System.Enum.GetValues(typeof(ShapeType)).Length);
        offer.blockType = (BlockType)Random.Range(0, System.Enum.GetValues(typeof(BlockType)).Length);
        offer.price = GetPriceByBlockType(offer.blockType);

        return offer;
    }

    private int GetPriceByBlockType(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.Attack:
                return attackPrice;
            case BlockType.Income:
                return incomePrice;
            case BlockType.Heal:
                return healPrice;
        }

        return 10;
    }

    private void UpdateOfferUI()
    {
        SetOfferUI(currentOffers[0], offerText1, offerImage1);
        SetOfferUI(currentOffers[1], offerText2, offerImage2);
        SetOfferUI(currentOffers[2], offerText3, offerImage3);
    }

    private void SetOfferUI(ShopOffer offer, TMP_Text textUI, Image imageUI)
    {
        if (offer == null) return;

        if (textUI != null)
        {
            textUI.text = FormatOfferInfo(offer);
        }

        if (imageUI != null)
        {
            imageUI.sprite = GetShapeSprite(offer.shapeType);
            imageUI.preserveAspect = true;
            imageUI.enabled = imageUI.sprite != null;
        }
    }

    private string FormatOfferInfo(ShopOffer offer)
    {
        string color = "#FFFFFF";

        switch (offer.blockType)
        {
            case BlockType.Attack:
                color = "#FF5A5A";
                break;
            case BlockType.Income:
                color = "#FFD84D";
                break;
            case BlockType.Heal:
                color = "#63E88E";
                break;
        }

        return $"<color={color}>{offer.blockType}</color>\n${offer.price}";
    }

    private Sprite GetShapeSprite(ShapeType shapeType)
    {
        foreach (ShapeSpriteEntry entry in shapeSprites)
        {
            if (entry.shapeType == shapeType)
            {
                return entry.sprite;
            }
        }

        return null;
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
        if (block.GetComponent<BlockVFX>() == null)
        {
            block.AddComponent<BlockVFX>();
        }

        AttackBlock oldAttack = block.GetComponent<AttackBlock>();
        IncomeBlock oldIncome = block.GetComponent<IncomeBlock>();
        HealBlock oldHeal = block.GetComponent<HealBlock>();

        if (oldAttack != null) Destroy(oldAttack);
        if (oldIncome != null) Destroy(oldIncome);
        if (oldHeal != null) Destroy(oldHeal);

        switch (blockType)
        {
            case BlockType.Attack:
            {
                AttackBlock attackBlock = block.AddComponent<AttackBlock>();
                attackBlock.damage = attackDamage;
                attackBlock.attackInterval = attackInterval;
                attackBlock.attackRange = attackRange;
                ApplyBlockMaterial(block, attackMaterial);
                break;
            }

            case BlockType.Income:
            {
                IncomeBlock incomeBlock = block.AddComponent<IncomeBlock>();
                incomeBlock.goldAmount = incomeGoldAmount;
                incomeBlock.incomeInterval = incomeInterval;
                ApplyBlockMaterial(block, incomeMaterial);
                break;
            }

            case BlockType.Heal:
            {
                HealBlock healBlock = block.AddComponent<HealBlock>();
                healBlock.healAmount = healAmount;
                healBlock.healInterval = healInterval;
                ApplyBlockMaterial(block, healMaterial);
                break;
            }
        }
    }

    private void ApplyBlockMaterial(GameObject block, Material targetMaterial)
    {
        if (targetMaterial == null) return;

        MeshRenderer[] renderers = block.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.sharedMaterial = targetMaterial;
            }
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold;
        }
    }

    private void UpdateButtonStates()
    {
        bool blockedByGameOver = GameStateManager.Instance != null && GameStateManager.Instance.IsGameOver;
        bool blocked = blockedByGameOver || !purchaseUnlocked;

        SetButtonState(offerButton1, currentOffers[0], blocked);
        SetButtonState(offerButton2, currentOffers[1], blocked);
        SetButtonState(offerButton3, currentOffers[2], blocked);
    }

    private void SetButtonState(Button button, ShopOffer offer, bool blocked)
    {
        if (button == null || offer == null) return;

        bool canAfford = gold >= offer.price;
        button.interactable = !blocked && canAfford;
    }
}
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;  
    public static GameManager Instance => instance;

    [SerializeField] private int allFood;
    //[SerializeField] private int totalFood; 
    //[SerializeField] private int totalGrill;

    [SerializeField] private LevelDataSO levelData;


    [SerializeField] private Transform gridGrill;

    [SerializeField] private CanvasGroup rewardGroup;
    [SerializeField] private RectTransform trayReward;
    [SerializeField] private Image[] rewardFoods;
    [SerializeField] private RectTransform[] traySlots;

    [SerializeField] private UILevelController uiLevelController;

    private List<GrillStation> listGrills;
    private float avgTray;
    private List<Sprite> totalSpriteFood;

    private void Awake()
    {
        listGrills = Utils.GetListInChild<GrillStation>(gridGrill);
        Sprite[] loadedSprite = Resources.LoadAll<Sprite>("Items");
        totalSpriteFood = loadedSprite.ToList();
        instance = this;
    }

    private void Start()
    {
        InitLevelFromSO(levelData);
        uiLevelController.Init(levelData);
    }

    private void InitLevelFromSO(LevelDataSO level)
    {
        var board = level.boardData;
        var spawn = level.spawnWareData;

        if (board == null || spawn == null)
        {
            Debug.LogError("LevelDataSO thiếu boardData hoặc spawnWareData");
            return;
        }

        List<Sprite> spawnFoods = GenerateFoodList(spawn);

        if (spawn.listLayerData == null || spawn.listLayerData.Count == 0)
        {
            Debug.LogError("spawnWareData không có layerData");
            return;
        }

        LayerSpawnData layer = spawn.listLayerData[0];

        int grillCount = Mathf.Min(listGrills.Count, board.grills.Count);

        int slotPerGrill = listGrills[0].TotalSlots.Count;

        DistributeFoodByLayer(spawnFoods, grillCount, slotPerGrill, layer, out var foodsOnGrillByGrill, out var foodsInTraysByGrill);

        for (int i = 0; i < grillCount; i++)
        {
            //var trayData = board.listTrayData[i];

            //if (trayData.trayType == TrayType.None)
            //{
            //    listGrills[i].gameObject.SetActive(false);
            //    continue;
            //}

            listGrills[i].gameObject.SetActive(true);

            List<Sprite> foodsOnGrill = foodsOnGrillByGrill[i];
            List<Sprite> foodsInTray = foodsInTraysByGrill[i];

            int trayCount = Mathf.CeilToInt((float)foodsInTray.Count / slotPerGrill);

            //trayCount = Mathf.Min(trayCount, Mathf.Max(0, trayData.size));

            listGrills[i].OnInitGrillFromLevel(foodsOnGrill, foodsInTray, trayCount);
        }

        // tắt grill dư
        for (int i = grillCount; i < listGrills.Count; i++)
            listGrills[i].gameObject.SetActive(false);

        allFood = spawnFoods.Count;

    }

    private List<Sprite> GenerateFoodList(SpawnWareDataSO spawn)
    {
        List<Sprite> result = new List<Sprite>();

        Sprite[] loadedSprite = Resources.LoadAll<Sprite>("Items");
        List<Sprite> pool = loadedSprite.ToList();

        // 1. chọn unique food
        List<Sprite> uniqueFoods = pool
            .OrderBy(x => Random.value)
            .Take(spawn.uniqueFoodCount)
            .ToList();

        if (uniqueFoods.Count < spawn.uniqueFoodCount)
        {
            Debug.LogError("Không đủ sprite food trong Resources/Items");
            return result;
        }

        // 2. tạo pattern
        for (int i = 0; i < spawn.totalPattern; i++)
        {
            Sprite s;

            if (i < uniqueFoods.Count)
            {
                // 
                s = uniqueFoods[i];
            }
            else
            {
                // phần dư → random trùng
                s = uniqueFoods[Random.Range(0, uniqueFoods.Count)];
            }

            result.Add(s);
            result.Add(s);
            result.Add(s);
        }

        // 3. shuffle
        for (int i = 0; i < result.Count; i++)
        {
            int r = Random.Range(i, result.Count);
            (result[i], result[r]) = (result[r], result[i]);
        }

        return result;
    }

    private void DistributeFoodByLayer( List<Sprite> allFoods, int grillCount, int slotPerGrill,LayerSpawnData layer, 
        out List<List<Sprite>> foodsOnGrillByGrill, out List<List<Sprite>> foodsInTraysByGrill )
    {
        foodsOnGrillByGrill = new List<List<Sprite>>();
        foodsInTraysByGrill = new List<List<Sprite>>();

        for (int i = 0; i < grillCount; i++)
        {
            foodsOnGrillByGrill.Add(new List<Sprite>());
            foodsInTraysByGrill.Add(new List<Sprite>());
        }

        int totalSlots = grillCount * slotPerGrill;

        // 1️⃣ Tổng food được phép nằm trên grill
        int maxFoodOnGrill = Mathf.Clamp(
            totalSlots - layer.numberEmptySlot,
            grillCount,                
            allFoods.Count
        );

        // 2️⃣ Mỗi grill có ít nhất 1 food
        int[] foodCountPerGrill = new int[grillCount];
        for (int i = 0; i < grillCount; i++)
            foodCountPerGrill[i] = 1;

        int remainingFood = maxFoodOnGrill - grillCount;

        // 3️⃣ Random phân phối food còn lại
        List<int> grillOrder = Enumerable.Range(0, grillCount)
            .OrderBy(_ => Random.value)
            .ToList();

        int gi = 0;
        while (remainingFood > 0)
        {
            int g = grillOrder[gi % grillCount];

            if (foodCountPerGrill[g] < slotPerGrill)
            {
                foodCountPerGrill[g]++;
                remainingFood--;
            }

            gi++;
        }

        // 4️⃣ Gán food 
        int foodIndex = 0;
        for (int g = 0; g < grillCount; g++)
        {
            for (int i = 0; i < foodCountPerGrill[g]; i++)
            {
                if (foodIndex >= allFoods.Count)
                    break;

                foodsOnGrillByGrill[g].Add(allFoods[foodIndex]);
                foodIndex++;
            }
        }

        // 5️ Food dư → tray
        int trayIdx = 0;
        while (foodIndex < allFoods.Count)
        {
            foodsInTraysByGrill[trayIdx % grillCount]
                .Add(allFoods[foodIndex]);

            foodIndex++;
            trayIdx++;
        }
    }


    //private List<int> DistributeEvelyn(int grillCount, int totalTrays)
    //{
    //    List<int> result = new List<int>();

    //    float avg = (float)totalTrays / grillCount;
    //    int low = Mathf.FloorToInt(avg);
    //    int high = Mathf.CeilToInt(avg);

    //    int highCount = totalTrays - low * grillCount;
    //    int lowCount = grillCount - highCount;

    //    for (int i = 0; i < lowCount; i++)
    //    {
    //        result.Add(low);
    //    }

    //    for (int i = 0; i < highCount; i++)
    //    {
    //        result.Add(high);
    //    }

    //    for(int i = 0; i< result.Count; i++)
    //    {
    //        int rand = Random.Range(i, result.Count);
    //        (result[i], result[rand]) = (result[rand], result[i]);
    //    }

    //    return result;
    //}

    public void OnMinusFood()
    {
        --allFood;
        if(allFood <= 0)
        {
            Debug.Log("Level Complete!");
        }
    }

    public void OnCheckAndShake()
    {
        Dictionary<string, List<FoodSlot>> dictFood = new Dictionary<string, List<FoodSlot>>();
        foreach(var grill in listGrills)
        {
            if (grill.gameObject.activeInHierarchy)
            {
                for(int i = 0; i< grill.TotalSlots.Count; i++)
                {
                    FoodSlot slot = grill.TotalSlots[i];
                    if (slot.HasFood)
                    {
                        string name = slot.GetSpriteFood.name;
                        if(!dictFood.ContainsKey(name))
                        {
                            dictFood.Add(name, new List<FoodSlot>());
                        }
                        dictFood[name].Add(slot);
                    }
                }
            }
        }
        foreach(var pair in dictFood)
        {
            List<FoodSlot> slots = pair.Value;
            if(slots.Count >= 3)
            {
                for(int i =0; i<3; i++)
                {
                    slots[i].DoShake();
                }
                return;
            }
        }
    }

    public List<GrillStation> GetAllActiveGrills()
    {
        return listGrills
            .Where(g => g != null && g.gameObject.activeInHierarchy)
            .ToList();
    }



    public void OnMergeReward(List<FoodSlot> slots)
    {
        if (slots == null || slots.Count < 3) return;

        StopAllCoroutines();
        StartCoroutine(IERewardTraySimple(slots.Take(3).ToList()));
    }

    private IEnumerator IERewardTraySimple(List<FoodSlot> foods)
    {
        //
        ShowTrayWithEffect();
        yield return new WaitForSeconds(0.15f);

        // 
        for (int i = 0; i < 3; i++)
        {
            FoodSlot slot = foods[i];
            Image img = rewardFoods[i];

            img.gameObject.SetActive(true);
            img.sprite = slot.GetSpriteFood;
            img.SetNativeSize();

            img.transform.SetParent(trayReward, true);
            img.transform.position = slot.transform.position;
            img.transform.localScale = Vector3.one;
   
            slot.OnActiveFood(false);
        }

        yield return null;

        // 
        for (int i = 0; i < 3; i++)
        {
            Image img = rewardFoods[i];
            RectTransform targetSlot = traySlots[i];

            img.transform.DOLocalMove(targetSlot.localPosition, 0.45f)
                .SetEase(Ease.InOutSine);

            img.transform.DOScale(0.6f, 0.45f)
                .SetEase(Ease.InOutSine);


            yield return new WaitForSeconds(0.12f);
        }
  
        yield return new WaitForSeconds(0.5f);

        float outX = trayReward.anchoredPosition.x + trayReward.rect.width + 150f;

        trayReward.DOAnchorPosX(outX, 0.35f)
            .SetEase(Ease.InCubic);

        yield return new WaitForSeconds(0.35f);

        for (int i = 0; i < 3; i++)
            rewardFoods[i].gameObject.SetActive(false);

        trayReward.gameObject.SetActive(false);
    }

    private void ShowTrayWithEffect()
    {
        trayReward.gameObject.SetActive(true);

        float offScreenX = trayReward.rect.width + 50f;
        float targetX = -trayReward.rect.width * 0.5f;
        
        trayReward.anchoredPosition =
            new Vector2(offScreenX, trayReward.anchoredPosition.y);

        trayReward
            .DOAnchorPosX(targetX, 0.35f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                trayReward.DOShakeAnchorPos(
                    0.25f,
                    new Vector2(8f, 0f),
                    10
                );
            });
    }


}


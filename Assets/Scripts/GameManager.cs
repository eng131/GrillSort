using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;  
    public static GameManager Instance => instance;

    [SerializeField] private int allFood;
    [SerializeField] private int totalFood; 
    [SerializeField] private int totalGrill;
    [SerializeField] private Transform gridGrill;

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

    void Start()
    {
        OnInitLevel();
    }

    private void OnInitLevel()
    {
        List<Sprite> takeFood = totalSpriteFood.OrderBy(x => Random.value).Take(totalFood).ToList();
        List<Sprite> useFood = new List<Sprite>();

        for (int i = 0; i < allFood ; i++)
        {
            int n = i % takeFood.Count;

            for (int j = 0; j < 3; j++)
            {
                useFood.Add(takeFood[n]);
            }
        }

        //for (int i = 0; i < useFood.Count; i++)
        //{
        //    int rand = Random.Range(i, useFood.Count);
        //    (useFood[i], useFood[rand]) = (useFood[rand], useFood[i]);
        //}

        avgTray = Random.Range(1.5f, 2f);
        int totalTray = Mathf.RoundToInt(useFood.Count / avgTray);

        List<int> trayPerGrill = this.DistributeEvelyn(totalGrill, totalTray);
        List<int> foodPerGrill = this.DistributeEvelyn(totalGrill, useFood.Count);

        for (int i = 0; i < listGrills.Count; i++)
        {
            bool activeGrill = i < totalGrill;
            listGrills[i].gameObject.SetActive(activeGrill);

            if (activeGrill)
            {
                List<Sprite> listFood = Utils.TakeAndRemoveRandom<Sprite>(useFood, foodPerGrill[i]);
                listGrills[i].OnInitGrill(trayPerGrill[i], listFood);
            }
        }
    }

    private List<int> DistributeEvelyn(int grillCount, int totalTrays)
    {
        List<int> result = new List<int>();

        float avg = (float)totalTrays / grillCount;
        int low = Mathf.FloorToInt(avg);
        int high = Mathf.CeilToInt(avg);

        int highCount = totalTrays - low * grillCount;
        int lowCount = grillCount - highCount;

        for (int i = 0; i < lowCount; i++)
        {
            result.Add(low);
        }

        for (int i = 0; i < highCount; i++)
        {
            result.Add(high);
        }

        for(int i = 0; i< result.Count; i++)
        {
            int rand = Random.Range(i, result.Count);
            (result[i], result[rand]) = (result[rand], result[i]);
        }

        return result;
    }

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

}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
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
    }

    void Start()
    {
        OnInitLevel();
    }

    private void OnInitLevel()
    {
        List<Sprite> takeFood = totalSpriteFood.OrderBy(x => Random.value).Take(totalFood).ToList();
        List<Sprite> useFood = new List<Sprite>();
        for (int i = 0; i < takeFood.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                useFood.Add(takeFood[i]);
            }
        }

        for (int i = 0; i < useFood.Count; i++)
        {
            int rand = Random.Range(i, useFood.Count);
            (useFood[i], useFood[rand]) = (useFood[rand], useFood[i]);
        }

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
}

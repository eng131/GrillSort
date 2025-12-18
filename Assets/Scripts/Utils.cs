using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{

    public static List<T> GetListInChild<T>(Transform parent)
    {
        List<T> list = new List<T>();
        for (int i = 0; i < parent.childCount; i++)
        {
            var component = parent.GetChild(i).GetComponent<T>();
            if (component!=null)
            {
                list.Add(component);
            }
        }
        return list;
    }

    public static List<T> TakeAndRemoveRandom<T>(List<T> list, int n)
    {
        List<T> res =new List<T>();
        n = Mathf.Min(n, list.Count);

        for(int i =0; i<n; i++)
        {
            int ranIndex = Random.Range(0, list.Count);
            res.Add(list[ranIndex]);
            list.RemoveAt(ranIndex);
        }

        return res;
    }
}

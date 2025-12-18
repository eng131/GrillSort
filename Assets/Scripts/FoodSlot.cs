using UnityEngine;
using UnityEngine.UI;

public class FoodSlot : MonoBehaviour
{
    private Image imgFood;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        imgFood = this.transform.GetChild(0).GetComponent<Image>();
        imgFood.gameObject.SetActive(false);
    }

    public void OnSetSlot(Sprite sprite)
    {
        imgFood.gameObject.SetActive(true);
        imgFood.sprite = sprite;
        imgFood.SetNativeSize();
    }

    public bool HasFood => imgFood.gameObject.activeInHierarchy;
}
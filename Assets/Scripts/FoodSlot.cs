using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FoodSlot : MonoBehaviour
{
    private Image imgFood;

    private Color normalColor = new Color(1f, 1f, 1f, 1f);
    private Color fadeColor = new Color(1f, 1f, 1f, 0.7f);

    private GrillStation grillCtrl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        imgFood = this.transform.GetChild(0).GetComponent<Image>();
        imgFood.gameObject.SetActive(false);
        grillCtrl = this.transform.parent.parent.GetComponent<GrillStation>();
    }

    public void OnSetSlot(Sprite sprite)
    {
        imgFood.gameObject.SetActive(true);
        imgFood.sprite = sprite;
        imgFood.SetNativeSize();
    }

    public void onActiveFood(bool active)
    {
        imgFood.gameObject.SetActive(active);
        imgFood.color = normalColor;
    }

    public void OnFadeFood()
    {
        this.onActiveFood(true);
        imgFood.color = fadeColor;
    }

    public void OnHideFood()
    {
        this.onActiveFood(false);
        imgFood.color = normalColor;
    }

    public void OnCheckMerge()
    {
        grillCtrl?.OnCheckMerge();
    }

    public void OnPrepareItem(Image img)
    {
        this.OnSetSlot(img.sprite);
        imgFood.color = normalColor;
        imgFood.transform.position = img.transform.position;
        imgFood.transform.localScale = img.transform.localScale;
        imgFood.transform.localEulerAngles = img.transform.localEulerAngles;

        imgFood.transform.DOLocalMove(Vector3.zero, 0.2f);
        imgFood.transform.DOScale(Vector3.one, 0.2f);
        imgFood.transform.DORotate(Vector3.zero, 0.2f);
    }

    public void OnCheckPrepareTray()
    {
        grillCtrl?.OnCheckPrepareTray();
    }

    public void DoShake()
    {
        imgFood.transform.DOShakePosition(0.5f, 10f, 10, 180f);
    }

    public FoodSlot GetSlotNull => grillCtrl.GetSlotNull();

    public bool HasFood => imgFood.gameObject.activeInHierarchy && imgFood.color == normalColor;

    public Sprite GetSpriteFood => imgFood.sprite;
}
using UnityEngine;
using UnityEngine.UI;

public class UIBoosterButton : MonoBehaviour
{
    public enum BoosterType { Magnet, Shuffle, Magic }
    public BoosterType type;

    public void OnClick()
    {
        switch (type)
        {
            case BoosterType.Magnet:
                BoosterManager.Instance.UseMagnet();
                break;
            case BoosterType.Shuffle:
                BoosterManager.Instance.UseShuffle();
                break;
            case BoosterType.Magic:
                BoosterManager.Instance.UseMagic();
                break;
        }
    }
}

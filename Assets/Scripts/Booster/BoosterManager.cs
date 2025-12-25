using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance;

    public MagnetBooster magnet;
    public ShuffleBooster shuffle;
    public MagicBooster magic;

    private void Awake()
    {
        Instance = this;
    }

    public void UseMagnet()
    {
        if (magnet.CanUse())
            magnet.Execute();
    }

    public void UseShuffle()
    {
        if (shuffle.CanUse())
            shuffle.Execute();
    }

    public void UseMagic()
    {
        if (magic.CanUse())
            magic.Execute();
    }
}

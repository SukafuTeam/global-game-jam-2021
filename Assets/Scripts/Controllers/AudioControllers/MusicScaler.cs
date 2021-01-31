using DG.Tweening;
using UnityEngine;

public class MusicScaler : MonoBehaviour
{
    private Vector3 _initialScale;

    void Start()
    {
        _initialScale = transform.localScale;
    }
    
    void Update()
    {
        if (Conductor.Instance.IsBeat)
        {
            transform.DOPunchScale(_initialScale * 0.2f, 0.2f, 10, 0.05f);
        }
    }
}

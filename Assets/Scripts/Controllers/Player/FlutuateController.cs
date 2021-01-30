using DG.Tweening;
using UnityEngine;

public class FlutuateController : MonoBehaviour
{
    public float FloatSpeed;
    public float FloatMagnitude;

    // Start is called before the first frame update
    void Start()
    {
        var seq = DOTween.Sequence();
        var pos = transform.position;
        pos.y -= FloatMagnitude;
        transform.position = pos;
        pos.y += FloatMagnitude * 2;

        var speed = FloatSpeed * Random.Range(0.7f, 1.3f);
        
        seq.Append(transform.DOMove(pos, speed).SetEase(Ease.InOutQuad));
        pos.y -= FloatMagnitude * 2;
        seq.Append(transform.DOMove(pos, speed).SetEase(Ease.InOutQuad));
        seq.SetLoops(-1);
        seq.Play();
    }
}

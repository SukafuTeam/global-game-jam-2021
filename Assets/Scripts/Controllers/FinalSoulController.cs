using System.Collections;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class FinalSoulController : MonoBehaviour
{
    public float AnimationTime;
    public float FinalSoulDelay;
    
    public CinemachineDollyCart Guider1;
    public CinemachineDollyCart Guider2;
    public CinemachineDollyCart Guider3;
    
    public GameObject FinalSoul;
    
    public SpriteRenderer Fade;
    
    void Start()
    {
        DOTween.To(() => Guider1.m_Position, x => Guider1.m_Position = x, 1, AnimationTime)
            .SetEase(Ease.OutCubic);
        DOTween.To(() => Guider2.m_Position, x => Guider2.m_Position = x, 1, AnimationTime)
            .SetEase(Ease.OutCubic);
        DOTween.To(() => Guider3.m_Position, x => Guider3.m_Position = x, 1, AnimationTime)
            .SetEase(Ease.OutCubic);

        StartCoroutine(SpawnFinalSoul());
    }

    public IEnumerator SpawnFinalSoul()
    {
        var obj = Instantiate(FinalSoul, new Vector3(0, 1.5f, 0), Quaternion.identity);
        yield return new WaitForSeconds(FinalSoulDelay);
        obj.transform.DOMoveY(10, AnimationTime - FinalSoulDelay).SetEase(Ease.InOutCubic);
        
        yield return new WaitForSeconds(AnimationTime - FinalSoulDelay - 8f);

        Fade.DOFade(1, 5f);
    }
}

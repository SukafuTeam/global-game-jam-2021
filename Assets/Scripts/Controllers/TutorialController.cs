using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public SpriteRenderer TutorialBaloon;

    public Sprite Watch;
    public Sprite Turn;
    public Sprite Play;
    
    // Start is called before the first frame update
    void Start()
    {
        TutorialBaloon.transform.localScale = Vector3.zero;
    }
    
    public void SetTutorial(int index)
    {
        TutorialBaloon.transform.DOKill();
        switch (index)
        {
            case 1:
                TutorialBaloon.transform.localScale = Vector3.zero;
                TutorialBaloon.sprite = Watch;
                TutorialBaloon.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                break;
            case 2:
                TutorialBaloon.transform.localScale = Vector3.zero;
                TutorialBaloon.sprite = Turn;
                TutorialBaloon.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                break;
            case 3:
                TutorialBaloon.transform.localScale = Vector3.zero;
                TutorialBaloon.sprite = Play;
                TutorialBaloon.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                break;
            case 4:
                TutorialBaloon.transform.localScale = Vector3.one;
                TutorialBaloon.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad);
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WaitingRoomController : MonoBehaviour
{
    public GameObject Enemyholder;
    public GameObject Player;
    public GameObject Button;
    
    // Start is called before the first frame update
    void Start()
    {
        var main = MainController.Instance;
        var enemy = Instantiate(main.Enemies[main.CompletedLevel], Enemyholder.transform.position, Quaternion.identity, Enemyholder.transform);
        StartSequence(Enemyholder, enemy, -3f);
        StartSequence(Player, Player, 3f);

        Button.transform.localScale = Vector3.zero;
        Button.transform.DOScale(Vector3.one, 2f)
            .SetDelay(2f)
            .SetEase(Ease.OutCubic);
    }

    public void StartSequence(GameObject refe, GameObject obj, float xOffset)
    {
        var sprite = obj.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0);
        sprite.DOFade(1, 2f);
        
        var Originalpos = refe.transform.position;
        var pos = refe.transform.position;
        pos.x += xOffset;
        refe.transform.position = pos;
        refe.transform.DOMove(Originalpos, 2f).SetEase(Ease.OutCubic);
    }
}

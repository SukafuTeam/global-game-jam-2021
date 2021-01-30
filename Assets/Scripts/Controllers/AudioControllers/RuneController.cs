using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public enum IndicatorState
{
    Neutral,
    Correct,
    Wrong
}

public class RuneController : MonoBehaviour
{
    public PentagramController Pentagram;
    public int StartBeat;
    public int EndBeat;

    public RuneData Data;

    public bool PreventNext;
    public bool isRunning;
    public bool IsFinished;
    public bool IsEnemy;
    
    public IndicatorState[] State = new IndicatorState[8];

    public GameObject ArrowPrefab;

    private Vector3 _initialScale;

    public void Start()
    {
        var seq = DOTween.Sequence();

        var xPos = IsEnemy ? -1 : 1;
        seq.Append(transform.DOMove(new Vector3(xPos, 3.5f, 0), 0.5f).SetEase(Ease.InOutQuad));
        seq.AppendInterval(1f);
        seq.Append(transform.DOMove(Pentagram.StartPoint.position, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => { StartCoroutine(StartRune()); })
        );
        seq.Play();

        _initialScale = transform.localScale;
    }
    
    public IEnumerator StartRune()
    {
        while (!Conductor.Instance.IsBeat)
            yield return null;

        if(IsEnemy)
            SoundController.PlaySfx(Data.EnemyClip);
        
        StartBeat = Conductor.Instance.lastBeat;
        EndBeat = Conductor.Instance.lastBeat + 1 + (Pentagram.NDivisions/2);
        
       isRunning = true;

       for (var i = 0; i < Pentagram.NDivisions; i++)
       {
           Pentagram.BeatIndicators[i].SetActive(Data.Options[i].Active);
       }
    }
    
    void Update()
    {
        BeatScale();
        
        if (!isRunning)
            return;
        
        var percentPosition =
            Mathf.InverseLerp((float) StartBeat, (float) EndBeat, Conductor.Instance.songPositionInBeats);
        var desiredPosition = Vector3.Lerp(Pentagram.StartPoint.position, Pentagram.Endpoint.position, percentPosition);

        transform.position = desiredPosition;

        if (IsEnemy)
        {
            CheckEnemyInput();
        }
        else
        {
            CheckInput();
            checkMissedBeats();
        }

        CheckFinish();
    }

    void BeatScale()
    {
        if (Conductor.Instance.IsBeat && !IsFinished)
        {
            transform.DOPunchScale(_initialScale * 0.05f, 0.2f, 10, 0.05f);
        }
    }
    
    void CheckInput()
    {
        // TODO: if this happens we should add an error to the user and early return
        //if(InputController.MultiPress) 
        
        if(!InputController.AnyPress)
            return;

        var currentTime = Conductor.Instance.songPositionInBeats;
        for (var i = 0; i < Pentagram.NDivisions + 2; i++)
        {
            var currentIndicator = i - 2;
            
            // We skip the first two half steps
            if(i==0 || i==1)
                continue;
            
            if(!Data.Options[currentIndicator].Active)
                continue;
            
            var minThreshold = StartBeat + 1f + (0.5f * currentIndicator) - 0.2f;
            var maxThreshold = StartBeat + 1f + (0.5f * currentIndicator) + 0.2f;

            var correctTiming = currentTime > minThreshold && currentTime < maxThreshold;
            if(!correctTiming)
                continue;
            
            bool correctPress;
            switch (Data.Options[currentIndicator].Direction)
            {
                case Direction.Up:
                    correctPress = InputController.Up;
                    break;
                case Direction.Right:
                    correctPress = InputController.Right;
                    break;
                case Direction.Down:
                    correctPress = InputController.Down;
                    break;
                case Direction.Left:
                    correctPress = InputController.Left;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (correctPress)
            {
                State[currentIndicator] = IndicatorState.Correct;
                var originalScale = Pentagram.BeatIndicators[currentIndicator].transform.localScale;
                Pentagram.BeatIndicators[currentIndicator].transform.DOPunchScale(originalScale * 1.1f, 0.3f, 5, 0.5f);
            }
            else
            {
                State[currentIndicator] = IndicatorState.Wrong;
                Pentagram.BeatIndicators[currentIndicator].transform.localScale *= 0.5f;
            }
        }
    }

    void checkMissedBeats()
    {
        var currentTime = Conductor.Instance.songPositionInBeats;
        for (var i = 0; i < Pentagram.NDivisions + 2; i++)
        {
            var currentIndicator = i - 2;
            
            // We skip the first two half steps
            if(i==0 || i==1)
                continue;
            
            if(!Data.Options[currentIndicator].Active)
                continue;
            
            var threshold = StartBeat + 1f + (0.5f * currentIndicator) + 0.2f;
            if (currentTime > threshold && State[currentIndicator] == IndicatorState.Neutral)
            {
                State[currentIndicator] = IndicatorState.Wrong;
                Pentagram.BeatIndicators[currentIndicator].transform.localScale *= 0.5f;
            }
        }
    }

    void CheckEnemyInput()
    {
        var currentTime = Conductor.Instance.songPositionInBeats;
        for (var i = 0; i < Pentagram.NDivisions + 2; i++)
        {
            var currentIndicator = i - 2;
            
            // We skip the first two half steps
            if(i==0 || i==1)
                continue;
            
            if(!Data.Options[currentIndicator].Active)
                continue;
            
            var threshold = StartBeat + 1f + (0.5f * currentIndicator);
            if (currentTime > threshold && State[currentIndicator] == IndicatorState.Neutral)
            {
                State[currentIndicator] = IndicatorState.Correct;
                var indicatorTransform = Pentagram.BeatIndicators[currentIndicator].transform;
                var originalScale = indicatorTransform.localScale;
                var originalPos = indicatorTransform.position;
                indicatorTransform.DOPunchScale(originalScale * 1.1f, 0.3f, 5, 0.5f);
                var arrow = Instantiate(ArrowPrefab, originalPos + new Vector3(0, -0.5f, 0), Quaternion.identity, Pentagram.ArrowsParent);
                var zRot = 0f; // down
                switch (Data.Options[currentIndicator].Direction)
                {
                    case Direction.Up:
                        zRot = 180f;
                        break;
                    case Direction.Left:
                        zRot = 270f;
                        break;
                    case Direction.Right:
                        zRot = 90;
                        break;
                }
                arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, zRot));
            }
        }
    }

    void CheckFinish()
    {
        if (IsFinished)
            return;

        var currentTime = Conductor.Instance.songPositionInBeats;
        
        if(currentTime < EndBeat)
            return;

        IsFinished = true;

        if(!PreventNext)
            GameController.Instance.CreateRune();
        
        transform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                Pentagram.ResetIndicators();
                Destroy(gameObject);
            });
    }

}

using R3;
using R3.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class R3Test : MonoBehaviour
{
    [SerializeField] private Text myText;

    void Start()
    {
        SubjectTest();
        DoubleClick();
    }

    private void UpdateObservable()
    {
        Observable.
            EveryUpdate(UnityFrameProvider.FixedUpdate, destroyCancellationToken).
            Subscribe(_ =>
            {
                Debug.Log("Fixed Update");
            });

        this.UpdateAsObservable().
            Subscribe(_ =>
            {
                Debug.Log("Update");
            });
    }

    private void DoubleClick()
    {
        var clickStream = this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0));

        clickStream.Chunk(clickStream.Debounce(TimeSpan.FromMilliseconds(200)))
            .Where(x => x.Count() >= 2)
            .SubscribeToText(myText, x => $"Double Clicked! Click Count = {x.Count()}");
    }

    private void SubjectTest()
    {
        Subject<string> subject= new Subject<string>();

        subject.Subscribe(msg => Debug.Log("Subscribe 1 " + msg));
        subject.Subscribe(msg => Debug.Log("Subscribe 2 " + msg));
        subject.Subscribe(msg => Debug.Log("Subscribe 3 " + msg));

        subject.OnNext("Hello");
        subject.OnNext("HI");
    }
}
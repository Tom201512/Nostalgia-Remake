using UnityEngine;
using System;
using System.Timers;
using ReelSpinGame_Observing;
using ReelSpinGame_Subject;

public class SubjectManager : MonoBehaviour
{
    private Subject _subject;
    private Observing _observing;

    private void Awake()
    {
        _subject = new Subject();
        _observing = new Observing(_subject);
    }

    private void Start()
    {
        _subject.DoThing();
    }
}

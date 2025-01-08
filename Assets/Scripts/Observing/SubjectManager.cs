using ReelSpinGame_Observing;
using ReelSpinGame_Subject;
using UnityEngine;

public class SubjectManager : MonoBehaviour
{
    private Subject subject;
    private Observing observing;

    private void Awake()
    {
        subject = new Subject();
        observing = new Observing(subject);
    }

    private void Start()
    {
        subject.DoThingA();
        subject.DoThingB();
    }
}

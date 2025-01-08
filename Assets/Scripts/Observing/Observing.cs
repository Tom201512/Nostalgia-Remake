using ReelSpinGame_Subject;
using UnityEngine;

namespace ReelSpinGame_Observing
{
    public class Observing
    {
        private readonly Subject subjectToObserve;

        public Observing(Subject _subject)
        {
            this.subjectToObserve = _subject;
            subjectToObserve.ThingHappenedA += OnThingHappenedA;
            subjectToObserve.ThingHappenedB += OnThingHappenedB;
        }

        ~Observing()
        {
            if (subjectToObserve != null)
            {
                subjectToObserve.ThingHappenedA -= OnThingHappenedA;
                subjectToObserve.ThingHappenedB -= OnThingHappenedB;
            }
        }

        private void OnThingHappenedA()
        {
            // A�C�x���g�ɉ������郍�W�b�N�͂����ɒu��
            Debug.Log("Observer responds A");
        }

        private void OnThingHappenedB()
        {
            // B�C�x���g�ɉ������郍�W�b�N�͂����ɒu��
            Debug.Log("Observer responds B");
        }
    }
}

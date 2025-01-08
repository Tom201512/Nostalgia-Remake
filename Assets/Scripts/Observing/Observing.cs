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
            // Aイベントに応答するロジックはここに置く
            Debug.Log("Observer responds A");
        }

        private void OnThingHappenedB()
        {
            // Bイベントに応答するロジックはここに置く
            Debug.Log("Observer responds B");
        }
    }
}

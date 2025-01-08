using ReelSpinGame_Interface;
using ReelSpinGame_Subject;
using UnityEngine;

namespace ReelSpinGame_Observing
{
    public class Observing
    {
        private ISubject subjectToObserve;

        public Observing(ISubject _subject)
        {
            this.subjectToObserve = _subject;
            subjectToObserve.ThingHappened += OnThingHappened;
        }

        ~Observing()
        {
            if (subjectToObserve != null)
            {
                subjectToObserve.ThingHappened -= OnThingHappened;
            }
        }

        private void OnThingHappened()
        {
            // イベントに応答するロジックはここに置く
            Debug.Log("Observer responds");
        }
    }
}

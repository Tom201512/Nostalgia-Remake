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
            // �C�x���g�ɉ������郍�W�b�N�͂����ɒu��
            Debug.Log("Observer responds");
        }
    }
}

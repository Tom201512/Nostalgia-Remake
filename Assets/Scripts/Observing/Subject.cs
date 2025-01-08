using ReelSpinGame_Interface;
using System;

namespace ReelSpinGame_Subject
{
    public class Subject : ISubject
    {
        public event Action ThingHappened;

        public void DoThing()
        {
            ThingHappened?.Invoke();
        }
    }
}

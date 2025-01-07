using System;

namespace ReelSpinGame_Subject
{
    public class Subject
    {
        public event Action ThingHappened;

        public void DoThing()
        {
            ThingHappened?.Invoke();
        }
    }
}

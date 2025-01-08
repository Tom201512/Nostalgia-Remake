using ReelSpinGame_Interface;
using System;

namespace ReelSpinGame_Subject
{
    public class Subject
    {
        public event Action ThingHappenedA;
        public event Action ThingHappenedB;

        public void DoThingA()
        {
            ThingHappenedA?.Invoke();
        }

        public void DoThingB()
        {
            ThingHappenedB?.Invoke();
        }
    }
}

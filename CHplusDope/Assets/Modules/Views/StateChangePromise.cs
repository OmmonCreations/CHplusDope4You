namespace Views
{
    public class StateChangePromise
    {
        public delegate void FulfillEvent();

        public event FulfillEvent Fulfilled;
        
        public bool IsFulfilled { get; private set; }

        public void Fulfill()
        {
            if (IsFulfilled) return;
            IsFulfilled = true;
            if (Fulfilled != null) Fulfilled();
        }
    }
}
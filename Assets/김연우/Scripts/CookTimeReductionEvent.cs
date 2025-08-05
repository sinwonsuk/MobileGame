public class CookTimeReductionEvent : IEvent
{
    public float reductionRate;
    public CookTimeReductionEvent(float rate)
    {
        reductionRate = rate;
    }
}

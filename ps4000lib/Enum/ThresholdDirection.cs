namespace PS4000Lib
{
    public enum ThresholdDirection : int
    {
        // Values for level threshold mode
        Above,
        Below,
        Rising,
        Falling,
        RisingOrFalling,

        // Values for window threshold mode
        Inside = Above,
        Outside = Below,
        Enter = Rising,
        Exit = Falling,
        EnterOrExit = RisingOrFalling,

        None = Rising,
    }
}

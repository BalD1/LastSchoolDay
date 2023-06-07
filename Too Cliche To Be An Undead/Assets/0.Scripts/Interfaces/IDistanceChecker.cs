public interface IDistanceChecker
{
    public abstract void OnEnteredFarCheck();
    public abstract void OnExitedFarCheck();

    public abstract void OnEnteredCloseCheck();
    public abstract void OnExitedCloseCheck();
}
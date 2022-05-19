namespace Client.Data;

public class CountIncrementService
{
    public int CurrentCount = 0;

    public void IncrementCount()
    {
        CurrentCount++;
    }
}

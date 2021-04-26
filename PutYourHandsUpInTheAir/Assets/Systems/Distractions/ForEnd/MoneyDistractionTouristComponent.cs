using Systems.Distractions;

public class MoneyDistractionTouristComponent : DistractedTouristComponent
{
    public float LastDistractionProgressTime { get; set; }
    public bool IsPaying { get; set; }
    public bool HasPaid { get; set; }
}

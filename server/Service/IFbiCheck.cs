namespace Service;

public interface IFbiCheck
{
    bool IsFbiBuyer();
}

public sealed class RandomFbiCheck : IFbiCheck
{
    public bool IsFbiBuyer() => Random.Shared.Next(100) < MarketplaceRules.FbiChancePercent;
}

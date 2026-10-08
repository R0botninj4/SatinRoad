namespace Service;

public static class MarketplaceRules
{
    // Whole percent: 1 means one chance in 100 successful purchases.
    public const int FbiChancePercent = 1;

    // A vendor is featured only when their order count is greater than this number.
    public const int FeaturedVendorSalesThreshold = 100;

    public const int LoyaltyDiscountInterval = 11;
    public const decimal LoyaltyDiscountMultiplier = 0.80m;
}

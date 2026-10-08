using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;

namespace Service;

public class ListingService(MyDatabaseConnection db)
{
    public List<ListingResponseDto> GetAll()
    {
        var usernames = db.Users
            .Select(user => new { user.Id, user.Username })
            .ToDictionary(user => user.Id, user => user.Username);

        var salesCounts = db.Orders
            .GroupBy(order => order.SellerId)
            .Select(group => new { SellerId = group.Key, Count = group.Count() })
            .ToDictionary(group => group.SellerId, group => group.Count);

        return db.Listings
            .OrderBy(listing => listing.Id)
            .ToList()
            .Select(listing => new ListingResponseDto
            {
                Id = listing.Id,
                UserId = listing.UserId,
                Username = usernames.GetValueOrDefault(listing.UserId) ?? "Unknown vendor",
                ItemId = listing.ItemId,
                Price = listing.Price,
                Quantity = listing.Quantity,
                Description = listing.Description,
                IsFeatured = salesCounts.GetValueOrDefault(listing.UserId)
                    > MarketplaceRules.FeaturedVendorSalesThreshold
            })
            .OrderByDescending(listing => listing.IsFeatured)
            .ThenBy(listing => listing.Price)
            .ToList();
    }

    public Listing Create(string userId, CreateListingRequestDto dto)
    {
        var user = db.Users.FirstOrDefault(user => user.Id == userId);

        if (user is null)
        {
            throw new ValidationException("User does not exist.");
        }

        if (user.IsShutDown)
        {
            throw new ValidationException("The FBI has shut down your shop. You cannot sell again.");
        }

        var itemExists = db.Items.Any(item => item.Id == dto.ItemId);

        if (!itemExists)
        {
            throw new ValidationException("Item does not exist.");
        }

        var listing = new Listing
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            ItemId = dto.ItemId,
            Price = dto.Price,
            Quantity = dto.Quantity,
            Description = dto.Description.Trim()
        };

        db.Insert(listing);

        return listing;
    }

    public bool Delete(string userId, string listingId)
    {
        var deletedRows = db.Listings
            .Where(listing => listing.Id == listingId && listing.UserId == userId)
            .Delete();

        return deletedRows > 0;
    }
}

using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;

namespace Service;

public class ListingService(MyDatabaseConnection db)
{
    public List<ListingResponseDto> GetAll()
    {
        var listings = db.Listings
            .OrderBy(listing => listing.Id)
            .ToList();

        return listings
            .Select(listing =>
            {
                var username = db.Users
                    .Where(user => user.Id == listing.UserId)
                    .Select(user => user.Username)
                    .FirstOrDefault();

                var numberOfSales = db.Orders.Count(order =>
                    order.SellerId == listing.UserId);

                return new ListingResponseDto
                {
                    Id = listing.Id,
                    UserId = listing.UserId,
                    Username = username ?? "Unknown vendor",
                    ItemId = listing.ItemId,
                    Price = listing.Price,
                    Quantity = listing.Quantity,
                    Description = listing.Description,
                    IsFeatured = numberOfSales > 100
                };
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

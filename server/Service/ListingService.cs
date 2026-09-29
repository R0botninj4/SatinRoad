using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;

namespace Service;

public class ListingService(MyDatabaseConnection db)
{
    public List<Listing> GetAll()
    {
        return db.Listings
            .OrderBy(listing => listing.Id)
            .ToList();
    }

    public Listing Create(string userId, CreateListingRequestDto dto)
    {
        var userExists = db.Users.Any(user => user.Id == userId);

        if (!userExists)
        {
            throw new ValidationException("User does not exist.");
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
}

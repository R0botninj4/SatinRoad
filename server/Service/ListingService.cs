using System.ComponentModel.DataAnnotations;
using Infra;

namespace Service;

public class ListingService(IApplicationData db)
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

                return new ListingResponseDto
                {
                    Id = listing.Id,
                    UserId = listing.UserId,
                    Username = username ?? "Unknown vendor",
                    ItemId = listing.ItemId,
                    Price = listing.Price,
                    Quantity = listing.Quantity,
                    Description = listing.Description
                };
            })
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

using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;

namespace Service;

public class OrderService(MyDatabaseConnection db)
{
    public Order Create(CreateOrderRequestDto dto)
    {
        var buyerExists = db.Users.Any(user => user.Id == dto.BuyerId);

        if (!buyerExists)
        {
            throw new ValidationException("Buyer does not exist.");
        }

        var listing = db.Listings
            .FirstOrDefault(listing => listing.Id == dto.ListingId);

        if (listing is null)
        {
            throw new ValidationException("Listing does not exist.");
        }

        if (listing.Quantity < dto.Quantity)
        {
            throw new ValidationException("Not enough items in stock.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            BuyerId = dto.BuyerId,
            ListingId = dto.ListingId,
            Quantity = dto.Quantity,
            TotalPrice = listing.Price * dto.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        listing.Quantity -= dto.Quantity;

        db.Insert(order);
        db.Update(listing);

        return order;
    }
}
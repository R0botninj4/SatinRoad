using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;

namespace Service;

public class OrderService(MyDatabaseConnection db)
{
    public Order Create(string buyerId, CreateOrderRequestDto dto)
    {
        var buyerExists = db.Users.Any(user => user.Id == buyerId);

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

        var remainingStock = OrderHelpers.CalculateRemainingStock(listing.Quantity, dto.Quantity);

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            BuyerId = buyerId,
            ListingId = dto.ListingId,
            ItemId = listing.ItemId,
            Quantity = dto.Quantity,
            TotalPrice = OrderHelpers.CalculateTotalPrice(listing.Price, dto.Quantity),
            CreatedAt = DateTime.UtcNow
        };

        listing.Quantity = remainingStock;

        db.Insert(order);
        if (listing.Quantity == 0)
        {
            db.Delete(listing);
        }
        else
        {
            db.Update(listing);
        }

        return order;
    }
    public List<Order> GetMyOrders(string buyerId)
    {
        return db.Orders
            .Where(order => order.BuyerId == buyerId)
            .OrderByDescending(order => order.CreatedAt)
            .ToList();
    }
}

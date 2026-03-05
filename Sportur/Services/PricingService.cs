using Sportur.Context;
using Sportur.Models;

namespace Sportur.Services
{
    public interface IPricingService
    {
        bool IsApprovedWholesaleUser(int? userId);
        decimal GetPrice(BicycleModel model, int? userId);
    }

    public class PricingService : IPricingService
    {
        private readonly SporturDbContext _context;

        public PricingService(SporturDbContext context)
        {
            _context = context;
        }

        public bool IsApprovedWholesaleUser(int? userId)
        {
            if (!userId.HasValue)
                return false;

            return _context.Users.Any(u =>
                u.Id == userId.Value &&
                u.Role == UserRole.Wholesale &&
                u.IsWholesaleApproved);
        }

        public decimal GetPrice(BicycleModel model, int? userId)
        {
            return IsApprovedWholesaleUser(userId)
                ? model.WholesalePrice
                : model.RetailPrice;
        }
    }
}


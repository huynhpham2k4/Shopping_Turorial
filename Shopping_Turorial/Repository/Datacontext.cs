using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models;

namespace Shopping_Tutorial.Repository
{
	public class Datacontext : IdentityDbContext<AppUserModel>
	{
		public Datacontext(DbContextOptions<Datacontext> options) : base(options)
		{

		}

		public DbSet<BrandModel> Brands { get; set; }
		public DbSet<SliderModel> Sliders { get; set; }
		public DbSet<ProductModel> Products { get; set; }
		public DbSet<CategoryModel> Categories { get; set; }

		public DbSet<OrderModel> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<RatingModel> Ratings { get; set; }
		public DbSet<ContactModel> Contacts { get; set; }
		public DbSet<CompareModel> Compares { get; set; }
		public DbSet<WishlistModel> Wishlists { get; set; }
		public DbSet<ProductQuantityModel> productQuantities { get; set; }
		public DbSet<ShippingModel> Shippings { get; set; }
		public DbSet<CouponModel> Coupons { get; set; }
		public DbSet<StatisticalModel> Statisticals { get; set; }
	}

}

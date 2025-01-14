namespace Price_Comparison_Website.Models
{
	public class Category
	{
		public Category()
		{
			Products = new List<Product>();
		}

		public int CategoryId { get; set; }  // PK
		public string? Name { get; set; }
		public ICollection<Product>? Products { get; set; }
	}
}

namespace Price_Comparison_Website.Models
{
	public class Category
	{
		public int CategoryId { get; set; }  // PK
		public string? Name { get; set; }
		public string? Description { get; set; }
		public ICollection<Product>? Products { get; set; }
	}
}

using System.ComponentModel.DataAnnotations.Schema;

// Đảm bảo namespace là MaChiPhuShoe.Models
namespace MaChiPhuShoe.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal Price { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}

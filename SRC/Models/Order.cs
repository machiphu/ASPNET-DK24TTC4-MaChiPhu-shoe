using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaChiPhuShoe.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Gán mặc định để tránh CS8618
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 0)")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } = null!; // EF sẽ gán -> dùng null-forgiving
    }
}

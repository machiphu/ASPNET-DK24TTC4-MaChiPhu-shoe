using System.ComponentModel.DataAnnotations;

namespace MaChiPhuShoe.Models.ViewModels
{
    // "Tờ đơn" này chỉ chứa đúng 3 thông tin cần thiết
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public required string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ giao hàng")]
        public required string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        public required string PhoneNumber { get; set; }
    }
}

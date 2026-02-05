using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaChiPhuShoe.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [Display(Name = "Tên sản phẩm")]
        [MaxLength(255)]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập giá bán.")]
        [Display(Name = "Giá bán")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Giá cũ (tùy chọn)")]
        public decimal? OldPrice { get; set; }

        [Display(Name = "Đường dẫn hình ảnh")]
        [MaxLength(1000)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Kích thước")]
        [MaxLength(50)]
        public string? Size { get; set; }

        [Display(Name = "Màu sắc")]
        [MaxLength(50)]
        public string? Color { get; set; }

        [Display(Name = "Mô tả sản phẩm")]
        public string? Description { get; set; }

        // --- THÔNG SỐ CHI TIẾT ---
        [Display(Name = "Tình trạng sản phẩm")]
        public string? ProductState { get; set; }

        [Display(Name = "Tình trạng kho")]
        public string? StockStatus { get; set; }

        [Display(Name = "Bảo hành")]
        public string? Warranty { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // ====== KHÓA NGOẠI DANH MỤC ======
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }           // FK
        public Category? Category { get; set; }       // Navigation
    }
}

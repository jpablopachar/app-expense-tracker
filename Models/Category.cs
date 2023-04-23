using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_expense_tracker.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string? Title { get; set; }
        [Column(TypeName = "nvarchar(5)")]
        public string? Icon { get; set; } = "";
        [Column(TypeName = "nvarchar(10)")]
        public string? Type { get; set; } = "Expense";
        [NotMapped]
        public string TitleWidthIcon {
            get {
                return $"{Icon} {Title}";
            }
        }
    }
}
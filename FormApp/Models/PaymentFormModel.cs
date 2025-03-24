using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FormApp.Models
{
    public class PaymentFormModel
    {
        // User information
        [Required(ErrorMessage = "ID is required")]
        [StringLength(9, MinimumLength = 1, ErrorMessage = "ID must be 9 numbers")]
        [RegularExpression(@"^\d+$", ErrorMessage = "ID must contain only numbers")]
        [DisplayName("ID")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last Name must be between 1 and 50 characters")]
        [DisplayName("Name")]
        [RegularExpression(@"^[^<>{}()\[\]\\\/;:=+*&^%$#@!?~|]+$", ErrorMessage = "Name contains characters that are not allowed")]
        public string UserLastName { get; set; }

        // Payment information
        [Required(ErrorMessage = "Please select an account type.")]
        public string SelectedAccount { get; set; }

        // Amount handling
        [RegularExpression(@"^\d+$", ErrorMessage = "Dollar must contain only numbers")]
        public int? DollarAmount { get; set; }

        [Range(0, 99, ErrorMessage = "Cents must be between 0 and 99.")] 
        [DataType(DataType.Text)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Cents must contain only numbers")]
        public int CentsAmount { get; set; } = 0;

        [Range(1, Int32.MaxValue, ErrorMessage = "Total Amount must be a Positive, Non-Zero value!")]
        // Computed properties 
        public int CentsTotal => (DollarAmount ?? 0) * 100 + CentsAmount;

        public decimal TotalAmount => CentsTotal / 100.0m;

    }
}

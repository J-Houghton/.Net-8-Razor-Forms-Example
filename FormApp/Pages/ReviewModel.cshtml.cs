using FormApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text.Json;

namespace FormApp.Pages
{
    public class ReviewModel : PageModel
    {
        private readonly ILogger<ReviewModel> _logger;

        private const string FORM_DATA_KEY = "FormData";
        private const string MESSAGE_KEY = "FormMessage"; 
        public PaymentFormModel FormData { get; private set; } = new PaymentFormModel(); 
        public string? Message { get; private set; } 

        public ReviewModel(ILogger<ReviewModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        { 
            if (TempData.Peek(FORM_DATA_KEY) is not string serializedData)
            {
                TempData[MESSAGE_KEY] = "Invalid WID, please search again.";
                return RedirectToPage("./Index");
            }
             
            FormData = JsonSerializer.Deserialize<PaymentFormModel>(serializedData);

            if (FormData.CentsTotal < 1)
            {
                TempData[MESSAGE_KEY] = "Invalid amount, please try again.";
                return RedirectToPage("./Index2");
            }

            Message = TempData[MESSAGE_KEY] as string;

            return Page();
        }

        // improvements:
        //   Implement security measures for the external redirect 
        public IActionResult OnPost()
        {
            if (TempData[FORM_DATA_KEY] is not string serializedData)
            {
                return RedirectToPage("./Index");
            }

            FormData = JsonSerializer.Deserialize<PaymentFormModel>(serializedData);

            /* 
               do something 
            */

            return Redirect("https://www.google.com");
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("./Index2");
        }
    }
}

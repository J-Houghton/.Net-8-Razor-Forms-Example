using FormApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;

/* .NET 8 Razor Pages: 
 *   - Basic Architecutre: Simplified MVC into a page based approach
 *      - PageModel, .cshtml.cs file, acts as both the Controller and Model container
 *          - OnGet, OnPost handlers - Controller
 *          - Properties defined in Page - Model
 *              - [BindProperty] attributes tell the framework to populate properties from form values
 *              - Can be defined directly in Page model, or separate model for more complex forms/data 
 *              
 *      - View, .cshtml file, HTML and razor syntax that renders the client 
 *          - ASP tags, such asp-for and asp-page-handler are essential to know
 *          
 *   - OnGet: 
 *      1. Server creates a new instance of Index
 *      2. Constructor runs 
 *      3. OnGet method runs on this instance
 *      4. Page is renders and sent to client 
 *      5. This instance is disposed. 
 *      
 *   - OnPost: 
 *      1. Server creates a NEW instance of Index
 *      2. Constructor runs
 *      3. The OnPost method runs on this instance
 *      4. The response is sent, OnPost() -> Redirect, OnPostReset() -> Same page
 *      5. This instance is disposed. 
 *      
 *   - Date Flow and State
 *      - TempData: For passing data between requests - Implemented here
 *      - Session: For storing user-specific data for longer periods
 *      - Cookies: For client-side storage
 *      - Database: For permanent storage
 *      
 *   For a better understanding, look into:
 *      - Routing (how URLs map to pages)
 *      - Page handlers with parameters (OnPostDelete(int id), etc.)
 *      - Tag Helpers beyond asp-for and asp-page-handler
 */

namespace FormApp.Pages
{ 
    public class Index : PageModel
    { 
        private readonly ILogger<Index> _logger;
        private const string FORM_DATA_KEY = "FormData";
        private const string MESSAGE_KEY = "FormMessage";

        [BindProperty]
        public PaymentFormModel FormData { get; set; } = new PaymentFormModel(); 

        public string? Message { get; set; } 

        public Index(ILogger<Index> logger) 
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ModelState.Remove("FormData.SelectedAccount");
            ModelState.Remove("FormData.DollarAmount");
            ModelState.Remove("FormData.CentsAmount");
            ModelState.Remove("FormData.CentsAmount");

            if (TempData.Peek(FORM_DATA_KEY) is string serializedData)
            {
                FormData = JsonSerializer.Deserialize<PaymentFormModel>(serializedData);
            }

            Message = TempData[MESSAGE_KEY] as string; 
        }
        //controller 
        public IActionResult OnPost()
        {
            ModelState.Remove("FormData.SelectedAccount"); 
            ModelState.Remove("FormData.DollarAmount");
            ModelState.Remove("FormData.CentsAmount");
            ModelState.Remove("FormData.CentsTotal");

            if (!ModelState.IsValid)
            {
                TempData[FORM_DATA_KEY] = JsonSerializer.Serialize(FormData);
                return Page();
            }

            FormData.UserId = Regex.Replace(FormData.UserId, @"[^\d]", "");
            FormData.UserLastName = Regex.Replace(FormData.UserLastName, @"[<>{}()\[\]\\\/;:=+*&^%$#@!?~|]", "");

            TempData[FORM_DATA_KEY] = JsonSerializer.Serialize(FormData);
            //Make API call, found or try again
            /*
                if found message = found redirect, send tempData
                else message = tryagain, same page, keep tempData
             */
            TempData[MESSAGE_KEY] = "Hello from index1";

            return RedirectToPage("./Index2");
        }

        public IActionResult OnPostReset()
        {  
            TempData.Remove(FORM_DATA_KEY);
            TempData.Remove(MESSAGE_KEY); 
            return RedirectToPage("./Index");
        }
    }
}
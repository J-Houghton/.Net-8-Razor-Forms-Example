using FormApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FormApp.Pages
{
    public class Index2 : PageModel 
    {
        private readonly ILogger<Index2> _logger;

        private const string FORM_DATA_KEY = "FormData";
        private const string MESSAGE_KEY = "FormMessage"; 

        [BindProperty]
        public PaymentFormModel FormData { get; set; } = new PaymentFormModel();

        public List<SelectListItem> AccountOptions { get; private set; }

        public string? Message { get; set; }

        public Index2(ILogger<Index2> logger)
        {
            _logger = logger;
            LoadAccountOptions();
        }

        public IActionResult OnGet()
        {
            if (TempData.Peek(FORM_DATA_KEY) is not string serializedData)
            {
                TempData[MESSAGE_KEY] = "Invalid WID, Please Search Again";
                return RedirectToPage("./Index");
            }

            FormData = JsonSerializer.Deserialize<PaymentFormModel>(serializedData);

            Message = TempData[MESSAGE_KEY] as string;

            return Page();
        } 

        public IActionResult OnPost()
        { 

            if (!ValidateAndSetUserData())
            {
                return RedirectToPage("./Index");
            }

            FormData.SelectedAccount = Regex.Replace(FormData.SelectedAccount, @"[^a-zA-Z0-9]", "");

            bool isValidAccount = AccountOptions.Any(option => option.Value == FormData.SelectedAccount);
            bool hasValidAmount = FormData.CentsTotal >= 1;

            if (!ModelState.IsValid || !isValidAccount || !hasValidAmount)
            {
                BuildSimpleErrorMessage(!isValidAccount, !hasValidAmount);
                return Page();
            }
             
            TempData[FORM_DATA_KEY] = JsonSerializer.Serialize(FormData);
            return RedirectToPage("./ReviewModel");
        }

        private bool ValidateAndSetUserData()
        {
            if (TempData.Peek(FORM_DATA_KEY) is not string serializedData) return false;

            PaymentFormModel formUserData = JsonSerializer.Deserialize<PaymentFormModel>(serializedData);

            if (string.IsNullOrEmpty(formUserData.UserId)) return false;

            if (string.IsNullOrEmpty(formUserData.UserLastName)) return false;

            ModelState.Remove("FormData.UserId");
            FormData.UserId = formUserData.UserId;
            ModelState.Remove("FormData.UserLastName");
            FormData.UserLastName = formUserData.UserLastName;

            return true;
        }

        private void BuildSimpleErrorMessage(bool accountError, bool amountError)
        {
            Message = "Please correct the following errors:";
            var errorsToAdd = new List<KeyValuePair<string, string>>();

            if (accountError)
            {
                errorsToAdd.Add(new KeyValuePair<string, string>("FormData.SelectedAccount", "Please select a valid account."));
                Message += "\nSelected account must be a valid option.";
            }

            if (amountError && !ModelState.ContainsKey("FormData.CentsTotal"))
            {
                errorsToAdd.Add(new KeyValuePair<string, string>("FormData.CentsTotal", "Total Amount Must Be Positive or Non-Zero!"));
            }

            foreach (var state in ModelState.Where(m => m.Value.Errors.Count > 0))
            {
                foreach (var error in state.Value.Errors)
                {  
                    Message += $"\n{error.ErrorMessage}";
                }
            }  
        }

        private void LoadAccountOptions()
        {
            // Dummy data for the select list
            AccountOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select Account Type --", Selected = true },
                new SelectListItem { Value = "Checking", Text = "Checking Account" },
                new SelectListItem { Value = "Savings", Text = "Savings Account" },
                new SelectListItem { Value = "Investment", Text = "Investment Account" },
                new SelectListItem { Value = "Retirement", Text = "Retirement Account" }
            };
        }
    }
}

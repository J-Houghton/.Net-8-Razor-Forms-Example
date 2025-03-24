using System.Text.RegularExpressions;

namespace FormApp
{
    /* 
     * This Validation class is a good example to set up a global Validation and Santization for many Pages
     *      
     *      Usage example, Index.cshtml.cs: 
     *          1. Uncomment program.cs to add the UserInputValidtor Service
     *              - builder.Services.AddScoped<IUserInputValidator, UserInputValidator>();
     *          2. Create _validator property
     *          3. Set _validator in Constructor
     *          4. Validator used Index.cshtml.cs's OnPost() 
     *          
     *  NOTE: To fully utilize this set up, consider using UserInputModel object or properties as the parameter
     *          Since this app contains 3 pages, with 4 inputs to check. A complex/custom model and validator is not need.
     *          
     */
    public interface IUserInputValidator
    {
        bool ValidateAndSanitizeId(ref string id, out string errorMessage);
        bool ValidateAndSanitizeLastName(ref string lastName, out string errorMessage);
        // Add more methods as needed
    }

    public class UserInputValidator : IUserInputValidator
    {
        public bool ValidateAndSanitizeId(ref string id, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Basic validation
            if (string.IsNullOrWhiteSpace(id))
            {
                errorMessage = "ID is required";
                return false;
            }

            // Remove any whitespace
            id = id.Trim();

            // Check if contains only digits
            if (!Regex.IsMatch(id, @"^\d+$"))
            {
                errorMessage = "ID must contain only numbers";
                return false;
            }
             
            //ID must be 9
            if (id.Length == 9)
            {
                errorMessage = "ID cannot must be 9 digits";
                return false;
            }

            return true;
        }

        public bool ValidateAndSanitizeLastName(ref string lastName, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Basic validation
            if (string.IsNullOrWhiteSpace(lastName))
            {
                errorMessage = "Last name is required";
                return false;
            }

            // Trim whitespace
            lastName = lastName.Trim();

            // Check for valid characters, this regex is very basic and should not be used for prod 
            if (!Regex.IsMatch(lastName, @"^[a-zA-Z\s\-'.]+$"))
            {
                errorMessage = "Last name contains invalid characters";
                return false;
            }

            // Additional validation/sanitization 

            // Normalize casing (optional)
            // lastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lastName.ToLower());

            return true;
        }
    }
}

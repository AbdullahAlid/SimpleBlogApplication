using System.ComponentModel.DataAnnotations;

namespace SimpleBlogApplication.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current Password is required.")]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password.")]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password.")]
        [Display(Name = "New Password")]
        [MinLength(8, ErrorMessage = "New Password must be at least 8 characters long.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm New Password is required.")]
        [DataType(DataType.Password, ErrorMessage = "Please enter a valid password.")]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}

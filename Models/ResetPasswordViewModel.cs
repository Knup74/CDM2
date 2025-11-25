namespace CDM.Models
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
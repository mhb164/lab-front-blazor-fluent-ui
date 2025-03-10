namespace FrontBlazorFluentUI.Dto;

public class ChangeLocalPasswordRequest
{
    public ChangeLocalPasswordRequest(string currentPassword, string newPassword)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }

    public  string CurrentPassword { get; set; }
    public  string NewPassword { get; set; }
}

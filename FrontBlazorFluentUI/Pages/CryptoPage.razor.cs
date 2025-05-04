namespace Laboratory.Front.Pages;

public partial class CryptoPage
{
    [Inject] public required LayoutService LayoutService { get; set; }
    [Inject] public required EncryptionService Encryption { get; set; } = default!;
    [Inject] public required IApiClient ApiClient { get; set; } = default!;

    public string Key { get; set; } = "Passwoed!";
    public string Message { get; set; } = "This command is intended to be used within the Package Manager Console in Visual Studio, as it uses the NuGet module's version of Install-Package. ";
    public string Encrypted { get; set; } = string.Empty;
    public string Encrypted_Server { get; set; } = string.Empty;
    public string Decrypted { get; set; } = string.Empty;
    public string Decrypted_Server { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        LayoutService.Title = "Crypto";
    }


    private async Task EncryptMessage()
    {
        if (string.IsNullOrWhiteSpace(Key) || string.IsNullOrWhiteSpace(Message))
            return;

        try
        {
            Encrypted = await Encryption.EncryptAsync(Message, Key);

        }
        catch (Exception ex) { Encrypted = ex.Message; }

        var serverResult = await ApiClient.CryptoAsync(new Shared.Crypto.CryptoRequest()
        {
            Content = Message,
            Password = Key,
            Iterations = 100_000,
            Operation = Shared.Crypto.CryptoOperation.Encrypt
        });

        Encrypted_Server = serverResult.IsSuccess ? serverResult.Value : serverResult.Message;
    }

    private async Task DecryptMessage()
    {
        if (string.IsNullOrWhiteSpace(Key) || string.IsNullOrWhiteSpace(Encrypted))
            return;
        try
        {
            Decrypted = await Encryption.DecryptAsync(Encrypted, Key);

        }
        catch (Exception ex) { Decrypted = ex.Message; }

        var serverResult = await ApiClient.CryptoAsync(new Shared.Crypto.CryptoRequest()
        {
            Content = Encrypted,
            Password = Key,
            Iterations = 100_000,
            Operation = Shared.Crypto.CryptoOperation.Decrypt
        });
        Decrypted_Server = serverResult.IsSuccess ? serverResult.Value : serverResult.Message;
    }
}

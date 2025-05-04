namespace Shared.Services;

public class CryptoService : ICryptoService
{
    private readonly IJSRuntime _js;

    public CryptoService(IJSRuntime jsRuntime)
    {
        _js = jsRuntime;
    }

    public async Task<byte[]> GenerateBytesAsync(int length)
        => await _js.InvokeAsync<byte[]>("cryptojs.generateBytes", length);

    public async Task<byte[]> EncryptGCMAsync(byte[] plaintextBytes, byte[] passwordBytes, byte[] salt, byte[] iv, int iterations)
        => await _js.InvokeAsync<byte[]>(
            "cryptojs.encryptGCM",
            plaintextBytes,
            passwordBytes,
            salt,
            iv,
            iterations
        );

    public async Task<byte[]> DecryptGCMAsync(byte[] encryptedBytes, byte[] passwordBytes, byte[] salt, byte[] iv, int iterations)
        => await _js.InvokeAsync<byte[]>(
            "cryptojs.decryptGCM",
            encryptedBytes,
            passwordBytes,
            salt,
            iv,
            iterations
        );
}

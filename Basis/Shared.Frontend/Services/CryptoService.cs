namespace Shared.Services;

public class CryptoService : ICryptoService
{
    private readonly IJSRuntime _js;

    public CryptoService(IJSRuntime jsRuntime)
    {
        _js = jsRuntime;
    }

    public async Task<byte[]> GenerateBytesAsync(int length)
    {
        var intArray = await _js.InvokeAsync<int[]>("cryptojs.generateBytes", length);
        return [.. intArray.Select(i => (byte)i)];
    }

    public async Task<byte[]> EncryptGCMAsync(byte[] plaintextBytes, byte[] passwordBytes, byte[] salt, byte[] iv, int iterations)
    {
        var intArray = await _js.InvokeAsync<int[]>(
            "cryptojs.encryptGCM",
            plaintextBytes,
            passwordBytes,
            salt,
            iv,
            iterations
        );
        return [.. intArray.Select(i => (byte)i)];
    }

    public async Task<byte[]> DecryptGCMAsync(byte[] encryptedBytes, byte[] passwordBytes, byte[] salt, byte[] iv, int iterations)
    {
        var intArray = await _js.InvokeAsync<int[]>(
            "cryptojs.decryptGCM",
            encryptedBytes,
            passwordBytes,
            salt,
            iv,
            iterations
        );
        return [.. intArray.Select(i => (byte)i)];
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SGENERGY.BusinessLayers.Services;

public class LocalMediaStorage
{
    private readonly IWebHostEnvironment _env;

    public LocalMediaStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<(string relativePath, string fileName, string contentType, long sizeBytes)> SaveAsync(IFormFile file, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var folder = Path.Combine(_env.WebRootPath, "uploads", now.Year.ToString(), now.Month.ToString("D2"));
        Directory.CreateDirectory(folder);

        var ext = Path.GetExtension(file.FileName);
        var safeName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, safeName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        var relativePath = $"/uploads/{now.Year}/{now.Month:D2}/{safeName}";
        return (relativePath, safeName, file.ContentType, file.Length);
    }
}
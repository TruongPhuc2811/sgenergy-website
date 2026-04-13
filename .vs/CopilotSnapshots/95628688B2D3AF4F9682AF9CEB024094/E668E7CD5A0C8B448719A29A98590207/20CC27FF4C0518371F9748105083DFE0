using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SGENERGY.DataLayers;
using SGENERGY.DomainModels.Entities;

namespace SGENERGY.BusinessLayers.Services;

public class MediaService
{
    private readonly IMediaRepository _mediaRepository;
    private readonly LocalMediaStorage _storage;

    public MediaService(IMediaRepository mediaRepository, LocalMediaStorage storage)
    {
        _mediaRepository = mediaRepository;
        _storage = storage;
    }

    public async Task<Media> CreateFromUploadAsync(IFormFile file, string? altText = null, string? title = null, CancellationToken ct = default)
    {
        if (file is null) throw new ArgumentNullException(nameof(file));

        var (relativePath, fileName, contentType, sizeBytes) = await _storage.SaveAsync(file, ct);

        var media = new Media
        {
            FilePath = relativePath,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            AltText = altText,
            Title = title,
            CreatedAt = DateTime.UtcNow
        };

        media.Id = await _mediaRepository.AddAsync(media, ct);
        return media;
    }
}
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SGENERGY.BusinessLayers
{
    /// <summary>
    /// Tiện ích tạo slug SEO-friendly từ chuỗi tiếng Việt hoặc tiếng Anh.
    /// </summary>
    public static class SlugHelper
    {
        // Bản đồ ký tự có dấu → không dấu (tiếng Việt)
        private static readonly Dictionary<char, char> VietnameseMap = new()
        {
            ['à'] = 'a', ['á'] = 'a', ['â'] = 'a', ['ã'] = 'a', ['ä'] = 'a',
            ['å'] = 'a', ['ă'] = 'a', ['ắ'] = 'a', ['ặ'] = 'a', ['ằ'] = 'a',
            ['ẳ'] = 'a', ['ẵ'] = 'a', ['ấ'] = 'a', ['ầ'] = 'a', ['ẩ'] = 'a',
            ['ẫ'] = 'a', ['ậ'] = 'a',
            ['è'] = 'e', ['é'] = 'e', ['ê'] = 'e', ['ë'] = 'e',
            ['ế'] = 'e', ['ề'] = 'e', ['ể'] = 'e', ['ễ'] = 'e', ['ệ'] = 'e',
            ['ì'] = 'i', ['í'] = 'i', ['î'] = 'i', ['ï'] = 'i',
            ['ò'] = 'o', ['ó'] = 'o', ['ô'] = 'o', ['õ'] = 'o', ['ö'] = 'o',
            ['ø'] = 'o', ['ơ'] = 'o', ['ớ'] = 'o', ['ợ'] = 'o', ['ờ'] = 'o',
            ['ở'] = 'o', ['ỡ'] = 'o', ['ố'] = 'o', ['ồ'] = 'o', ['ổ'] = 'o',
            ['ỗ'] = 'o', ['ộ'] = 'o',
            ['ù'] = 'u', ['ú'] = 'u', ['û'] = 'u', ['ü'] = 'u',
            ['ư'] = 'u', ['ứ'] = 'u', ['ự'] = 'u', ['ừ'] = 'u', ['ử'] = 'u',
            ['ữ'] = 'u',
            ['ý'] = 'y', ['ỳ'] = 'y', ['ỷ'] = 'y', ['ỹ'] = 'y', ['ỵ'] = 'y',
            ['đ'] = 'd',
            ['ñ'] = 'n',
        };

        /// <summary>
        /// Tạo slug SEO-friendly từ tên (hỗ trợ tiếng Việt).
        /// Ví dụ: "Tấm Pin Năng Lượng Mặt Trời LONGi 570 WP" → "tam-pin-nang-luong-mat-troi-longi-570-wp"
        /// </summary>
        /// <param name="name">Chuỗi cần chuyển thành slug</param>
        /// <returns>Slug chữ thường, không dấu, ngăn cách bằng dấu gạch ngang</returns>
        public static string GenerateSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            // Chuyển sang chữ thường
            var lower = name.ToLowerInvariant();

            // Thay thế ký tự tiếng Việt bằng ASCII tương đương
            var sb = new StringBuilder(lower.Length);
            foreach (char c in lower)
            {
                if (VietnameseMap.TryGetValue(c, out char mapped))
                    sb.Append(mapped);
                else
                    sb.Append(c);
            }

            // Normalize Unicode (xóa dấu còn sót lại sau NFD decomposition)
            var normalized = sb.ToString().Normalize(NormalizationForm.FormD);
            var asciiSb = new StringBuilder(normalized.Length);
            foreach (char c in normalized)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != UnicodeCategory.NonSpacingMark)
                    asciiSb.Append(c);
            }

            var result = asciiSb.ToString().Normalize(NormalizationForm.FormC);

            // Thay khoảng trắng và ký tự đặc biệt bằng dấu gạch ngang
            result = Regex.Replace(result, @"[^a-z0-9\s-]", "");
            result = Regex.Replace(result, @"[\s-]+", "-");
            result = result.Trim('-');

            return result;
        }

        /// <summary>
        /// Trả về slug duy nhất bằng cách thêm hậu tố "-1", "-2", … khi trùng.
        /// </summary>
        /// <param name="baseSlug">Slug cơ sở đã được tạo từ GenerateSlug</param>
        /// <param name="existsAsync">Hàm kiểm tra xem slug đã tồn tại hay chưa (trả về true nếu đã tồn tại)</param>
        /// <returns>Slug duy nhất</returns>
        public static async Task<string> MakeUniqueAsync(string baseSlug, Func<string, Task<bool>> existsAsync)
        {
            if (!await existsAsync(baseSlug))
                return baseSlug;

            int suffix = 1;
            string candidate;
            do
            {
                candidate = $"{baseSlug}-{suffix++}";
            }
            while (await existsAsync(candidate));

            return candidate;
        }
    }
}

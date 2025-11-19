namespace Application.Service.Helpers.Normalitazion
{
    public static class TextNormalization
    {
        public static string Normalization(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Trim().ToUpper();
        }
    }
}

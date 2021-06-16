namespace Sample8.Common
{
    public static class EnumExtensions
    {
        private static object GetMetadata(ContentType ct)
        {
            var type = ct.GetType();
            var info = type.GetMember(ct.ToString());
            if ((info != null) && (info.Length > 0))
            {
                object[] attrs = info[0].GetCustomAttributes(typeof(EnumMetadata), false);
                if ((attrs != null) && (attrs.Length > 0))
                {
                    return attrs[0];
                }
            }
            return null;
        }

        public static string ToValue(this ContentType ct)
        {
            var metadata = GetMetadata(ct);
            return (metadata != null) ? ((EnumMetadata)metadata).Value : ct.ToString();
        }

        public static bool IsText(this ContentType ct)
        {
            var metadata = GetMetadata(ct);
            return (metadata != null) ? ((EnumMetadata)metadata).IsText : true;
        }

        public static bool IsBinary(this ContentType ct)
        {
            var metadata = GetMetadata(ct);
            return (metadata != null) ? ((EnumMetadata)metadata).IsBinary : false;
        }
    }
}

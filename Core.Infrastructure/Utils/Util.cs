using System;
namespace Core.Infrastructure.Utils
{
    public static class Util
    {
        public static bool IsNull(object value)
        {
            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                return false;
            }
            return true;
        }

        public static bool IsInt(object value)
        {
            if (IsNull(value))
                return false;

            int data;
            return int.TryParse(value.ToString(), out data);
        }

		public static bool IsBool(object value)
		{
			if (IsNull(value))
				return false;

			bool data;
            return bool.TryParse(value.ToString(), out data);
		}

		public static bool IsDateTime(object value)
		{
			if (IsNull(value))
				return false;

            DateTime data;
            return DateTime.TryParse(value.ToString(), out data);
		}

        #region[Convert vn-VN to en-GB]
        private static readonly string[] VietnameseSigns = new string[]{
        "aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ", "ÍÌỊỈĨ", "đ", "Đ",
        "ýỳỵỷỹ", "ÝỲỴỶỸ"
    };

        public static string RemoveVietNamese(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
        #endregion
    }
}

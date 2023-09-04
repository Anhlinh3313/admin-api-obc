using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Business.Core.Helpers
{
    public static class StringHelper
    {
        // <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            return LevenshteinDistance.Compute(s, t);
        }
        public static string[] _REPLACES_LOCATION_NAME = {" ", "NUOC", "TINH", "THANHPHO", "QUAN", "PHUONG", "XA", "HUYEN", "TP", "TT", "COUNTRY", "CITY", "STATE", "PROVINCE", "DISTRICT", "WARD" };
        // <summary>
        /// GetBestMatches
        /// <param name="sources">source input</param>
        /// <param name="columnNameCompare">property of class to compare</param>
        /// <param name="columnNameResult">property of class to get when compare</param>
        /// <param name="search"></param>
        /// <param name="percentlimitCost">percent limist cost to compare</param>
        /// <param name="replaces">array string to replace of param search</param>
        /// </summary>
        public static object GetBestMatches<T>(IEnumerable<T> sources, string columnNameResult, string columnNameCompare, string search, uint? percentlimitCost = null, string[] replaces = null) where T : class
        {
            if (sources == null || sources.Count() == 0)
            {
                throw new System.ArgumentNullException(nameof(sources));
            }
            if (string.IsNullOrWhiteSpace(search))
            {
                throw new System.ArgumentNullException(nameof(search));
            }
            if (string.IsNullOrWhiteSpace(columnNameCompare))
            {
                throw new System.ArgumentNullException(nameof(columnNameCompare));
            }
            if (string.IsNullOrWhiteSpace(columnNameResult))
            {
                throw new System.ArgumentNullException(nameof(columnNameResult));
            }
            Type t = typeof(T);
            var tInstance = (T)Activator.CreateInstance(typeof(T));
            if (!tInstance.HasProp(columnNameCompare))
            {
                throw new System.ArgumentNullException(nameof(columnNameCompare) + " a property not exist in a class " + t.Name);
            }
            if (!tInstance.HasProp(columnNameResult))
            {
                throw new System.ArgumentNullException(nameof(columnNameResult) + " a property not exist in a class " + t.Name);
            }

            search = search.ToAscii().ToUpper();
            if (replaces != null && replaces.Count() > 0)
            {
                search = replaces.Aggregate(search, (c1, c2) => c1.Replace(c2, ""));
            }
            uint limitCost = (percentlimitCost ?? 0) != 0 ? (uint)(search.Length) * percentlimitCost.Value / 100 : 0;
            int index = 0;
            dynamic minDistance = new ExpandoObject();
            minDistance.Cost = 0;
            //minDistance.Id = 0;
            foreach (var source in sources)
            {
                string value = source.GetPropValue(columnNameCompare) + "";
                value = value.ToAscii().ToUpper();
                if (replaces != null && replaces.Count() > 0)
                {
                    value = replaces.Aggregate(value, (c1, c2) => c1.Replace(c2, ""));
                }
                int cost = LevenshteinDistance.Compute(value, search);

                if (index == 0 || minDistance.Cost > cost)
                {
                    minDistance.Cost = cost;
                    minDistance.Id = source.GetPropValue(columnNameResult);
                    index++;
                }
            }
            if (minDistance.Cost > limitCost && limitCost > 0)
            {
                return 0;
            }
            try
            {
                return minDistance.Id;
            }
            catch
            {

                Type type = (Type)t.GetPropType(columnNameResult);
                return Activator.CreateInstance(type);
            }
        }

        static Regex ConvertToUnsign_rg = null;
        public static string NoneUnicode(this string text)
        {
            if (ReferenceEquals(ConvertToUnsign_rg, null))
            {
                ConvertToUnsign_rg = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            }
            var temp = text.Normalize(NormalizationForm.FormD);
            return ConvertToUnsign_rg.Replace(temp, string.Empty).Replace("đ", "d").Replace("Đ", "D").ToLower();
        }

    }
    public static class LevenshteinDistance
    {
        // <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}

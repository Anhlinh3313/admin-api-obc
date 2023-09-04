﻿using System;
 using System.ComponentModel;
 using System.ComponentModel.DataAnnotations;
 using System.Linq;
 using System.Reflection;
 using Core.Infrastructure.Core;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Extensions
{
	public static class Extensions
	{
        public static int ToSafeInt(this string value)
        {
            return Util.IsInt(value) ? int.Parse(value) : 0;
        }

		public static bool ToSafeBool(this string value)
		{
            return Util.IsBool(value) ? bool.Parse(value) : false;
		}

		public static DateTime ToSafeDateTime(this string value)
		{
            return Util.IsDateTime(value) ? DateTime.Parse(value) : DateTime.Now;
		}

		/// <summary>
		/// Extension method to add pagination info to Response headers
		/// </summary>
		/// <param name="response"></param>
		/// <param name="currentPage"></param>
		/// <param name="itemsPerPage"></param>
		/// <param name="totalItems"></param>
		/// <param name="totalPages"></param>
		public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
		{
			var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

			response.Headers.Add("Pagination",
			   Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));
			// CORS
			response.Headers.Add("access-control-expose-headers", "Pagination");
		}

		public static void AddApplicationError(this HttpResponse response, string message)
		{
			response.Headers.Add("Application-Error", message);
			// CORS
			response.Headers.Add("access-control-expose-headers", "Application-Error");
		}

        public static string GetEnumDisplayName(this System.Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Name;
            else
                return value.ToString();
        }
        public static string GetDescription(System.Enum value)
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var descriptionAttribute =
                enumMember == null
                    ? default(DescriptionAttribute)
                    : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return
                descriptionAttribute == null
                    ? value.ToString()
                    : descriptionAttribute.Description;
        }
	}
}

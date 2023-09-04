using System;
namespace Core.Api.Core
{
	public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
	{
		public CustomLanguageManager()
		{
			AddTranslation("vi", "NotNullValidator", "'{PropertyName}' is required.");
		}
	}
}

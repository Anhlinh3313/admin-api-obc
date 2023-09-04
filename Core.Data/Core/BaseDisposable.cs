using System;
namespace Core.Data.Core
{
	public class BaseDisposable : IDisposable
	{
		private bool _isDisposed;

		~BaseDisposable()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_isDisposed && disposing)
			{
				DisposeCore();
			}

			_isDisposed = true;
		}

		protected virtual void DisposeCore()
		{
		}
	}
}

using System;
using System.Linq.Expressions;

namespace AvaloniaUI.PrintToPDF
{
internal static class Restorer
{
	public static IDisposable Create<T>(Expression<Func<T>> expression)
	{
		var originalValue = expression.Compile().Invoke();
		var parameter = Expression.Parameter(typeof(T));
		var restorer = Expression.Lambda<Action<T>>(
			Expression.Assign(expression.Body, parameter),
			parameter);
		return Create(originalValue, restorer.Compile());
	}

	public static IDisposable Create<T>(T originalValue, Action<T> restore)
	{
		return new Impl(() => restore(originalValue));
	}

	private sealed class Impl : IDisposable
	{
		private readonly Action _popper;

		public Impl(Action popper)
		{
			_popper = popper;
		}

		public void Dispose()
		{
			_popper();
		}
	}
}
}
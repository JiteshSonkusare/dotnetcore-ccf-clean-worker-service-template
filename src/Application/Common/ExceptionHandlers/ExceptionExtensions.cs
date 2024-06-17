using Application.Common.Exceptions;

namespace Application.Common.ExceptionHandlers;

public static class ExceptionExtensions
{
	public static GeneralApplicationException With(this Exception exception, params string?[] otherExceptionDetails)
	{
		var message = string.Join("\n", otherExceptionDetails) ?? string.Empty;
		return new(message, exception);
	}
}

public record ExceptionInfo(string? Source, string? Message, string StackTrace);
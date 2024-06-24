using Shared.Wrapper;

namespace Domain.Errors;

public static class ErrorDto
{
    public static class TasksErrors
    {
        public static readonly Error EventsNotFound = new(
            "Events.NotFound",
            "Events not found to send it to genesys!");
    }

    public static class GenesysEventError
    {
        public static readonly Error DatatableNameNotFound = new(
            "Datatable.Name.NotFound",
            "Specified datatable name not found!");
    }

    public static class AuthErrors
    {
        public static Error AuthTokenError(string error) => new(
            "AuthToken.Failed",
            error);

        public static Error AuthTokenException(string exception) => new(
            "AuthToken.Exception",
            exception);
    }

    public static class MQErrors
    {
        public static Error MQMessageError(int code, string message) => new(
            code.ToString(), message);
    }
}
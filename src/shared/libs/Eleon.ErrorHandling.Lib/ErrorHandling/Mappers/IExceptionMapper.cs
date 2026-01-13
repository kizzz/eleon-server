namespace Logging.Module.ErrorHandling.Mappers;

/// <summary>
/// Maps exceptions to HTTP status codes, application error codes, and titles.
/// </summary>
public interface IExceptionMapper
{
    /// <summary>
    /// Maps an exception to HTTP status code, application error code, and title.
    /// </summary>
    /// <param name="exception">The exception to map.</param>
    /// <returns>Tuple containing HTTP status code, application error code, and title.</returns>
    (int HttpStatus, string AppCode, string Title) Map(Exception exception);
}

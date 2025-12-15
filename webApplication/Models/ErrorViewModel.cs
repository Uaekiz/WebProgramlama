namespace webApplication.Models;

public class ErrorViewModel //It carries a RequestId to allow the error to be tracked
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

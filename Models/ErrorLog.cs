public class ErrorLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string? RequestId { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Path { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}

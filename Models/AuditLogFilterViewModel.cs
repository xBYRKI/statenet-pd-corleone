using System;
using System.Collections.Generic;
using statenet_lspd.Models;

namespace statenet_lspd.ViewModels;

public class AuditLogFilterViewModel
{
    public string? SearchTerm { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public int PageNumber { get; set; }
    public int TotalPages { get; set; }

    public List<AuditLog> Logs { get; set; } = new();
}

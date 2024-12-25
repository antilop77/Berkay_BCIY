using System;
using System.Collections.Generic;

namespace Data;

public partial class ProcessLog
{
    public int Id { get; set; }

    public int? ProcessId { get; set; }

    public string? ReqPath { get; set; }

    public string? ReqBody { get; set; }

    public string? ResBody { get; set; }

    public string? LogType { get; set; }

    public DateTime? InsertDateTime { get; set; }
}

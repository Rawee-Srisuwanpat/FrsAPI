using System;
using System.Collections.Generic;

namespace field_recording_api.DataAccess.FieldRecording;

public partial class TbtFetchAlloc
{
    public long Id { get; set; }

    public string? FileName { get; set; }

    public string? UserName { get; set; }

    public DateTime? SyncDate { get; set; }

    public string? ActiveFlag { get; set; }

    public DateTime? DateTimeStamp { get; set; }
}

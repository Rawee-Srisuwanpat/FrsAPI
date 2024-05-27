using System;
using System.Collections.Generic;

namespace field_recording_api.DataAccess.FieldRecording;

public partial class TbtContractTermination
{
    public long Id { get; set; }

    public string ContractNo { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public DateTime? LastPayDate { get; set; }

    public DateTime? Timestamp { get; set; }
}

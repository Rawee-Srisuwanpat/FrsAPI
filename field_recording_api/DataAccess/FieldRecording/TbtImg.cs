using System;
using System.Collections.Generic;

namespace field_recording_api.DataAccess.FieldRecording;

public partial class TbtImg
{
    public long Id { get; set; }

    public string? ContractNo { get; set; }

    public string? ImgName { get; set; }

    public string? ImgPath { get; set; }

    public DateTime? DateTimeStamp { get; set; }

    public long? ContactNoteId { get; set; }

    public int? ImgSeq { get; set; }
}

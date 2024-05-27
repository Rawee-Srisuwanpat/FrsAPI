using System;
using System.Collections.Generic;

namespace field_recording_api.DataAccess.FieldRecording;

public partial class TbtContactNote
{
    public long ContactNoteId { get; set; }

    public string ContractNo { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string Result { get; set; } = null!;

    public string? NextAction { get; set; }

    public DateTime? NextActionDate { get; set; }

    public string? DelinquencyReason { get; set; }

    public string? Remarks { get; set; }

    public string? ModeOfContact { get; set; }

    public string? PlaceOfContact { get; set; }

    public string? PartyContacted { get; set; }

    public string? ContactNumber { get; set; }

    /// <summary>
    /// Lat,Long
    /// </summary>
    public string Geo { get; set; } = null!;

    public string? SubDistrict { get; set; }

    public string? District { get; set; }

    public string? Province { get; set; }

    public string? ZipCode { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    /// <summary>
    /// Y : Sync Complete
    /// N : No
    /// </summary>
    public string? IsSync { get; set; }

    public DateTime? SyncDate { get; set; }

    public string? SyncErrorMessage { get; set; }

    public string? IPAddress { get; set; }

    public DateTime? PTP_DT { get; set; }

    public Decimal? PTP_AMT { get; set; }

    public Decimal? OS_AMT { get; set; }

    public Decimal? OD_AMT { get; set; }


}

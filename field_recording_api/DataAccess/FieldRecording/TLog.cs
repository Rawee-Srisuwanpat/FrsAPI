using System;
using System.Collections.Generic;

namespace field_recording_api.DataAccess.FieldRecording;

public partial class TLog
{
    public Guid Id { get; set; }

    public string? ControllerName { get; set; }

    public string? ActionName { get; set; }

    public string? MethodName { get; set; }

    public string? InputData { get; set; }

    public string? OutputData { get; set; }

    public string? MessageCode { get; set; }

    public string? MessageThaDesc { get; set; }

    public string? MessageEngDesc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public Guid? TransId { get; set; }

    public string? Remark { get; set; }

    public string? ContractNo { get; set; }

    public string? UserName { get; set; }
}

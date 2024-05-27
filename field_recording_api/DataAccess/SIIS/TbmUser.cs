using System;
using System.Collections.Generic;

namespace field_recording_api.DataAccess.SIIS;

public partial class TbmUser
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserPass { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public int CreateBy { get; set; }

    public DateTime UpdateDate { get; set; }

    public int UpdateBy { get; set; }

    public int ActiveFlag { get; set; }

    public DateTime LastChangePassDate { get; set; }

    public int? OpenCloseScrLogFlag { get; set; }
}

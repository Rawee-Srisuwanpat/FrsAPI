using System;
using System.Collections.Generic;
using field_recording_api.Models.ActionCodeModel;
using field_recording_api.Models.ConfigCloseSystem;
using field_recording_api.Models.Payment;
using field_recording_api.Models.ResultCodeModel;
using Microsoft.EntityFrameworkCore;

namespace field_recording_api.DataAccess.FieldRecording;

public partial class FieldRecordingContext : DbContext
{
    public FieldRecordingContext()
    {
    }

    public FieldRecordingContext(DbContextOptions<FieldRecordingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TLog> TLogs { get; set; }

    public virtual DbSet<TbmUser> TbmUsers { get; set; }

    public virtual DbSet<TbtContactNote> TbtContactNotes { get; set; }

    public virtual DbSet<TbtContractTermination> TbtContractTerminations { get; set; }

    public virtual DbSet<TbtFetchAlloc> TbtFetchAllocs { get; set; }

    public virtual DbSet<TbtImg> TbtImgs { get; set; }

    public virtual DbSet<TbmExceptUsers> TbmExceptUsers { get; set; }

    public virtual DbSet<ActionCodeDaoModel> t_mst_action_code { get; set; }

    public virtual DbSet<ResultCodeDaoModel> t_mst_result_code { get; set; }
    public virtual DbSet<ConfigCloseSystemDaoModel> tbt_schedule_woking { get; set; }

    public virtual DbSet<tbt_special_day> tbt_special_day { get; set; }


   



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__T_Log__3214EC073E93948A");

            entity.ToTable("T_Log");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ActionName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ContractNo)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.ControllerName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InputData)
                .HasMaxLength(4000)
                .IsUnicode(false);
            entity.Property(e => e.MessageCode)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.MessageEngDesc).HasMaxLength(500);
            entity.Property(e => e.MessageThaDesc).HasMaxLength(500);
            entity.Property(e => e.MethodName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.OutputData)
                .HasMaxLength(4000)
                .IsUnicode(false);
            entity.Property(e => e.Remark)
                .HasMaxLength(4000)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(35)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbmUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbm_user");

            entity.Property(e => e.ActiveFlag).HasColumnName("active_flag");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.LastChangePassDate)
                .HasColumnType("datetime")
                .HasColumnName("last_change_pass_date");
            entity.Property(e => e.OpenCloseScrLogFlag).HasColumnName("open_close_scr_log_flag");
            entity.Property(e => e.UpdateBy).HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPass)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_pass");
        });

        modelBuilder.Entity<TbtContactNote>(entity =>
        {
            entity.HasKey(e => e.ContactNoteId);

            entity.ToTable("tbt_contact_note");

            entity.HasIndex(e => e.ContractNo, "NonClusteredIndex-20230816-114952");

            entity.HasIndex(e => e.Action, "NonClusteredIndex-20230816-115005");

            entity.HasIndex(e => e.Result, "NonClusteredIndex-20230816-115016");

            entity.Property(e => e.ContactNoteId).HasColumnName("contact_note_id");
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.ContactNumber).HasMaxLength(50);
            entity.Property(e => e.ContractNo)
                .HasMaxLength(50)
                .HasColumnName("contract_no");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(50)
                .HasColumnName("Create_By");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Create_Date");
            entity.Property(e => e.DelinquencyReason).HasMaxLength(50);
            entity.Property(e => e.District).HasMaxLength(50);
            entity.Property(e => e.Geo)
                .HasMaxLength(50)
                .HasComment("Lat,Long");
            entity.Property(e => e.IsSync)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasComment("Y : Sync Complete\r\nN : No")
                .HasColumnName("IS_Sync");
            entity.Property(e => e.ModeOfContact).HasMaxLength(50);
            entity.Property(e => e.NextAction).HasMaxLength(50);
            entity.Property(e => e.NextActionDate).HasColumnType("datetime");
            entity.Property(e => e.PartyContacted).HasMaxLength(50);
            entity.Property(e => e.PlaceOfContact).HasMaxLength(50);
            entity.Property(e => e.Province).HasMaxLength(50);
            entity.Property(e => e.Remarks).HasMaxLength(50);
            entity.Property(e => e.Result).HasMaxLength(50);
            entity.Property(e => e.SubDistrict).HasMaxLength(50);
            entity.Property(e => e.SyncDate)
                .HasColumnType("datetime")
                .HasColumnName("Sync_Date");
            entity.Property(e => e.SyncErrorMessage)
                .HasMaxLength(4000)
                .HasColumnName("Sync_Error_Message");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(50)
                .HasColumnName("Update_By");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("Update_Date");
            entity.Property(e => e.ZipCode).HasMaxLength(50);
        });

        modelBuilder.Entity<TbtContractTermination>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbt_contract_terminations");

            entity.Property(e => e.ContractNo)
                .HasMaxLength(30)
                .HasColumnName("contract_no");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IssueDate)
                .HasColumnType("datetime")
                .HasColumnName("issue_date");
            entity.Property(e => e.LastPayDate)
                .HasColumnType("datetime")
                .HasColumnName("last_pay_date");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbtFetchAlloc>(entity =>
        {
            entity.ToTable("tbt_fetchAlloc");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActiveFlag)
                .HasMaxLength(50)
                .HasColumnName("Active_flag");
            entity.Property(e => e.DateTimeStamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(50);
            entity.Property(e => e.SyncDate)
                .HasColumnType("datetime")
                .HasColumnName("Sync_Date");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<TbtImg>(entity =>
        {
            entity.ToTable("tbt_img");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ContactNoteId).HasColumnName("contact_note_id");
            entity.Property(e => e.ContractNo).HasMaxLength(50);
            entity.Property(e => e.DateTimeStamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImgName)
                .HasMaxLength(200)
                .HasColumnName("Img_Name");
            entity.Property(e => e.ImgPath)
                .HasMaxLength(500)
                .HasColumnName("Img_Path");
            entity.Property(e => e.ImgSeq).HasColumnName("img_seq");
        });

        modelBuilder.Entity<TbmExceptUsers>(entity =>
        {
            entity.ToTable("tbm_except_users");

            entity.Property(e => e.Id).HasColumnName("ID");
            
            entity.Property(e => e.USER_ID).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

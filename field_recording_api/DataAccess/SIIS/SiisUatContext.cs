using System;
using System.Collections.Generic;
using field_recording_api.Utilities;
using Microsoft.EntityFrameworkCore;

namespace field_recording_api.DataAccess.SIIS;

public partial class SiisUatContext : DbContext
{
    public SiisUatContext()
    {
    }

    public SiisUatContext(DbContextOptions<SiisUatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbmUser> TbmUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbmUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tbm_usr");

            entity.ToTable("tbm_user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ActiveFlag).HasColumnName("active_flag");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.LastChangePassDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_change_pass_date");
            entity.Property(e => e.OpenCloseScrLogFlag)
                .HasDefaultValueSql("((1))")
                .HasColumnName("open_close_scr_log_flag");
            entity.Property(e => e.UpdateBy).HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPass)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_pass");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

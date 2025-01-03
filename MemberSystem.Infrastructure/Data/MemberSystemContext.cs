using System;
using System.Collections.Generic;
using MemberSystem.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemberSystem.Infrastructure.Data;

public partial class MemberSystemContext : DbContext
{
    public MemberSystemContext()
    {
    }

    public MemberSystemContext(DbContextOptions<MemberSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApprovalFlow> ApprovalFlows { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<JobGrade> JobGrades { get; set; }

    public virtual DbSet<LeaveApproval> LeaveApprovals { get; set; }

    public virtual DbSet<LeaveBalance> LeaveBalances { get; set; }

    public virtual DbSet<LeavePolicy> LeavePolicies { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<LogDetail> LogDetails { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberDepartment> MemberDepartments { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:MemberSystemContext");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApprovalFlow>(entity =>
        {
            entity.HasKey(e => e.FlowId).HasName("PK__Approval__1184B35C51E3D1B4");

            entity.Property(e => e.FlowId).HasColumnName("FlowID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Department).WithMany(p => p.ApprovalFlows)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ApprovalF__Depar__078C1F06");

            entity.HasOne(d => d.Position).WithMany(p => p.ApprovalFlows)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ApprovalF__Posit__0880433F");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCDB465628C");

            entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC345CA10BCE").IsUnique();

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ParentDepartmentId).HasColumnName("ParentDepartmentID");

            entity.HasOne(d => d.ParentDepartment).WithMany(p => p.InverseParentDepartment)
                .HasForeignKey(d => d.ParentDepartmentId)
                .HasConstraintName("FK__Departmen__Paren__7B264821");
        });

        modelBuilder.Entity<JobGrade>(entity =>
        {
            entity.HasKey(e => e.JobGradeId).HasName("PK__JobGrade__4CED6BF5ADAFEA41");

            entity.HasIndex(e => e.GradeLevel, "UQ__JobGrade__FC44BC2BB6E111AA").IsUnique();

            entity.Property(e => e.JobGradeId).HasColumnName("JobGradeID");
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.GradeName).HasMaxLength(50);
            entity.Property(e => e.MaxSalary).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<LeaveApproval>(entity =>
        {
            entity.HasKey(e => e.ApprovalId).HasName("PK__LeaveApp__328477D4F51748FF");

            entity.Property(e => e.ApprovalId).HasColumnName("ApprovalID");
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.ApproverId).HasColumnName("ApproverID");
            entity.Property(e => e.FlowId).HasColumnName("FlowID");
            entity.Property(e => e.LeaveRequestId).HasColumnName("LeaveRequestID");

            entity.HasOne(d => d.Approver).WithMany(p => p.LeaveApprovals)
                .HasForeignKey(d => d.ApproverId)
                .HasConstraintName("FK__LeaveAppr__Appro__0E391C95");

            entity.HasOne(d => d.Flow).WithMany(p => p.LeaveApprovals)
                .HasForeignKey(d => d.FlowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveAppr__FlowI__0D44F85C");

            entity.HasOne(d => d.LeaveRequest).WithMany(p => p.LeaveApprovals)
                .HasForeignKey(d => d.LeaveRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveAppr__Leave__0C50D423");
        });

        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.HasKey(e => e.BalanceId).HasName("PK__LeaveBal__A760D59E43CB9DFA");

            entity.Property(e => e.RemainingDays).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveBalances)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveBala__Leave__58D1301D");

            entity.HasOne(d => d.Member).WithMany(p => p.LeaveBalances)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveBala__Membe__57DD0BE4");
        });

        modelBuilder.Entity<LeavePolicy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__LeavePol__2E133944F5D7A627");

            entity.Property(e => e.CarryOver).HasDefaultValue(false);
            entity.Property(e => e.ExpiryInMonths).HasDefaultValue(12);
            entity.Property(e => e.MinYearsOfService).HasDefaultValue(0);

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeavePolicies)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeavePoli__Leave__55009F39");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.LeaveRequestId).HasName("PK__LeaveReq__6094218E468DB59C");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LeaveType).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Approver).WithMany(p => p.LeaveRequestApprovers)
                .HasForeignKey(d => d.ApproverId)
                .HasConstraintName("FK_LeaveRequests_Members1");

            entity.HasOne(d => d.Member).WithMany(p => p.LeaveRequestMembers)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRequests_Members");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__LeaveTyp__43BE8FF4351BDFBF");

            entity.HasIndex(e => e.LeaveTypeName, "UQ__LeaveTyp__E6D8DFAB93272EBE").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsPaid).HasDefaultValue(true);
            entity.Property(e => e.LeaveTypeName).HasMaxLength(50);
            entity.Property(e => e.RequiresApproval).HasDefaultValue(true);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Logs__5E5499A846F36F61");

            entity.Property(e => e.LogTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LogType).HasMaxLength(50);
            entity.Property(e => e.RelatedSystem).HasMaxLength(100);
            entity.Property(e => e.Severity).HasMaxLength(20);

            entity.HasOne(d => d.Member).WithMany(p => p.Logs)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__Logs__MemberID__6442E2C9");
        });

        modelBuilder.Entity<LogDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__LogDetai__135C314D38C11D04");

            entity.HasOne(d => d.Log).WithMany(p => p.LogDetails)
                .HasForeignKey(d => d.LogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LogDetail__LogID__671F4F74");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Members__0CF04B1853814C34");

            entity.HasIndex(e => e.Username, "UQ__Members__536C85E49D209E97").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Members__A9D10534355CCB55").IsUnique();

            entity.Property(e => e.BloodType).HasMaxLength(2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.IsApproved).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Members)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Members_Roles");
        });

        modelBuilder.Entity<MemberDepartment>(entity =>
        {
            entity.HasKey(e => e.MemberDepartmentId).HasName("PK__MemberDe__65748ED22FD4808A");

            entity.ToTable("MemberDepartment");

            entity.Property(e => e.MemberDepartmentId).HasColumnName("MemberDepartmentID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Department).WithMany(p => p.MemberDepartments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MemberDep__Depar__02C769E9");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberDepartments)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MemberDep__Membe__01D345B0");

            entity.HasOne(d => d.Position).WithMany(p => p.MemberDepartments)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MemberDep__Posit__03BB8E22");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.PageName).HasMaxLength(50);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB0F093A3D10");

            entity.HasIndex(e => e.PermissionName, "UQ__Permissi__0FFDA3576A6385C4").IsUnique();

            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.PermissionName).HasMaxLength(50);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A59346DEF64");

            entity.HasIndex(e => e.PositionName, "UQ__Position__E46AEF4294F78772").IsUnique();

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.PositionName).HasMaxLength(50);

            entity.HasMany(d => d.Permissions).WithMany(p => p.Positions)
                .UsingEntity<Dictionary<string, object>>(
                    "PositionPermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PositionP__Permi__18B6AB08"),
                    l => l.HasOne<Position>().WithMany()
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PositionP__Posit__17C286CF"),
                    j =>
                    {
                        j.HasKey("PositionId", "PermissionId").HasName("PK__Position__8E41F5E9459A71C6");
                        j.ToTable("PositionPermissions");
                        j.IndexerProperty<int>("PositionId").HasColumnName("PositionID");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("PermissionID");
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A44AABF4B");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolePermi__Permi__14E61A24"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolePermi__RoleI__13F1F5EB"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__RolePerm__6400A18A02C2136A");
                        j.ToTable("RolePermissions");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("PermissionID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

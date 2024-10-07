using System;
using System.Collections.Generic;
using DataCloudDemo.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataCloudDemo.Models
{
    public partial class DCContext : DbContext
    {
        public DCContext()
        {
        }

        public DCContext(DbContextOptions<DCContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppConfig> AppConfigs { get; set; }
        public virtual DbSet<EventHistory> EventHistories { get; set; }
        public virtual DbSet<WebhookHistory> WebhookHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql(Utilities.GetPGConnectionString(Environment.GetEnvironmentVariable("DATABASE_URL")));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pg_stat_statements")
                .HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<AppConfig>(entity =>
            {
                entity.ToTable("app_configs", Utilities.DATABASE_SCHEMA);

                entity.HasIndex(e => e.ContactId, "idx_app_configs_contactid");

                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.ContactId)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.UserLocale).HasMaxLength(100);

                entity.Property(e => e.UserTimezone).HasMaxLength(100);
            });

            modelBuilder.Entity<EventHistory>(entity =>
            {
                entity.ToTable("event_history", Utilities.DATABASE_SCHEMA);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Contactid).HasColumnName("contactid");

                entity.Property(e => e.Contactupdatedon)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("contactupdatedon");

                entity.Property(e => e.Createdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createdate")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Dataactioninitiatedon)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("dataactioninitiatedon");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.Eventtype).HasColumnName("eventtype");

                entity.Property(e => e.Firstname).HasColumnName("firstname");

                entity.Property(e => e.Functioninvokedon)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("functioninvokedon");

                entity.Property(e => e.Lastname).HasColumnName("lastname");

                entity.Property(e => e.Platformeventinvokedon)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("platformeventinvokedon");
            });

            modelBuilder.Entity<WebhookHistory>(entity =>
            {
                entity.ToTable("webhook_history", Utilities.DATABASE_SCHEMA);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Contactid).HasColumnName("contactid");

                entity.Property(e => e.Contactupdatedon)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("contactupdatedon");

                entity.Property(e => e.Createdate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("createdate")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Dataactioninitiatedon)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("dataactioninitiatedon");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.Eventtype).HasColumnName("eventtype");

                entity.Property(e => e.Firstname).HasColumnName("firstname");

                entity.Property(e => e.Functioninvokedon)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("functioninvokedon");

                entity.Property(e => e.Lastname).HasColumnName("lastname");

                entity.Property(e => e.Platformeventinvokedon)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("platformeventinvokedon");
            });

            modelBuilder.HasSequence("event_history_id_seq", Utilities.DATABASE_SCHEMA);

            modelBuilder.HasSequence("webhook_history_id_seq", Utilities.DATABASE_SCHEMA);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

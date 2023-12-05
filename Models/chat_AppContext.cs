using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VedioChatApp_Server_.Models
{
    public partial class chat_AppContext : DbContext
    {
        public chat_AppContext()
        {
        }

        public chat_AppContext(DbContextOptions<chat_AppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Friendship> Friendships { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-1EU2VTK;Database=chat_App; Integrated Security=True; MultipleActiveResultSets=true; Trust Server Certificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.HasIndex(e => new { e.User1Id, e.User2Id }, "UniqueFriendship")
                    .IsUnique();

                entity.HasOne(d => d.User1)
                    .WithMany(p => p.FriendshipUser1s)
                    .HasForeignKey(d => d.User1Id)
                    .HasConstraintName("FK__Friendshi__User1__3F466844");

                entity.HasOne(d => d.User2)
                    .WithMany(p => p.FriendshipUser2s)
                    .HasForeignKey(d => d.User2Id)
                    .HasConstraintName("FK__Friendshi__User2__403A8C7D");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.SentAt).HasColumnType("datetime");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.MessageReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK__Messages__Receiv__49C3F6B7");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.MessageSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__Messages__Sender__48CFD27E");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.ConnectionId).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.SignalRid)
                    .HasMaxLength(10)
                    .HasColumnName("signalRId")
                    .IsFixedLength();

                entity.Property(e => e.Username).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

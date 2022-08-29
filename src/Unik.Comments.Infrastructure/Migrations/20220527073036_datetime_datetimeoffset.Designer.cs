﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Unik.Comments.Infrastructure.Contexts;

#nullable disable

namespace Unik.Comments.Infrastructure.Migrations
{
    [DbContext(typeof(CommentsDbContext))]
    [Migration("20220527073036_datetime_datetimeoffset")]
    partial class datetime_datetimeoffset
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Unik.Comments.Infrastructure.Models.CommentContentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<Guid>("InstanceId")
                        .HasColumnType("uniqueidentifier")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.HasKey("Id");

                    b.ToTable("CommentsContent", (string)null);
                });

            modelBuilder.Entity("Unik.Comments.Infrastructure.Models.CommentEntity", b =>
                {
                    b.Property<int>("InternalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InternalId"), 1L, 1);

                    b.Property<int>("ContentId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("InstanceId")
                        .HasColumnType("uniqueidentifier")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<Guid>("PublicId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.Property<int>("ThreadId")
                        .HasColumnType("int");

                    b.HasKey("InternalId");

                    b.HasIndex("ContentId")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("Comments", (string)null);
                });

            modelBuilder.Entity("Unik.Comments.Infrastructure.Models.CommentEntity", b =>
                {
                    b.HasOne("Unik.Comments.Infrastructure.Models.CommentContentEntity", "Content")
                        .WithOne("Comment")
                        .HasForeignKey("Unik.Comments.Infrastructure.Models.CommentEntity", "ContentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Unik.Comments.Infrastructure.Models.CommentEntity", "Parent")
                        .WithMany("Replies")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Content");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Unik.Comments.Infrastructure.Models.CommentContentEntity", b =>
                {
                    b.Navigation("Comment")
                        .IsRequired();
                });

            modelBuilder.Entity("Unik.Comments.Infrastructure.Models.CommentEntity", b =>
                {
                    b.Navigation("Replies");
                });
#pragma warning restore 612, 618
        }
    }
}

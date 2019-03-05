﻿// <auto-generated />
using CNCLib.Repository.SqLite;
using CNCLib.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace CNCLib.Repository.SqLite.Migrations
{
    [DbContext(typeof(CNCLibContext))]
    [Migration("20180112154650_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.Configuration", b =>
                {
                    b.Property<string>("Group")
                        .HasMaxLength(256);

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<int?>("UserId");

                    b.Property<string>("Value")
                        .HasMaxLength(4000);

                    b.HasKey("Group", "Name");

                    b.HasIndex("UserId");

                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<int?>("UserId");

                    b.HasKey("ItemId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.ItemProperty", b =>
                {
                    b.Property<int>("ItemId");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("Value");

                    b.HasKey("ItemId", "Name");

                    b.ToTable("ItemProperty");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.Machine", b =>
                {
                    b.Property<int>("MachineId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Axis");

                    b.Property<int>("BaudRate");

                    b.Property<int>("BufferSize");

                    b.Property<string>("ComPort")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<int>("CommandSyntax");

                    b.Property<bool>("CommandToUpper");

                    b.Property<bool>("Coolant");

                    b.Property<bool>("Laser");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<bool>("NeedDtr");

                    b.Property<decimal>("ProbeDist");

                    b.Property<decimal>("ProbeDistUp");

                    b.Property<decimal>("ProbeFeed");

                    b.Property<decimal>("ProbeSizeX");

                    b.Property<decimal>("ProbeSizeY");

                    b.Property<decimal>("ProbeSizeZ");

                    b.Property<bool>("Rotate");

                    b.Property<bool>("SDSupport");

                    b.Property<decimal>("SizeA");

                    b.Property<decimal>("SizeB");

                    b.Property<decimal>("SizeC");

                    b.Property<decimal>("SizeX");

                    b.Property<decimal>("SizeY");

                    b.Property<decimal>("SizeZ");

                    b.Property<bool>("Spindle");

                    b.Property<int?>("UserId");

                    b.HasKey("MachineId");

                    b.HasIndex("UserId");

                    b.ToTable("Machine");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.MachineCommand", b =>
                {
                    b.Property<int>("MachineCommandId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("JoystickMessage")
                        .HasMaxLength(64);

                    b.Property<int>("MachineId");

                    b.Property<int?>("PosX");

                    b.Property<int?>("PosY");

                    b.HasKey("MachineCommandId");

                    b.HasIndex("MachineId");

                    b.ToTable("MachineCommand");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.MachineInitCommand", b =>
                {
                    b.Property<int>("MachineInitCommandId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<int>("MachineId");

                    b.Property<int>("SeqNo");

                    b.HasKey("MachineInitCommandId");

                    b.HasIndex("MachineId");

                    b.ToTable("MachineInitCommand");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true);

                    b.Property<string>("UserPassword")
                        .HasMaxLength(255);

                    b.HasKey("UserId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.Configuration", b =>
                {
                    b.HasOne("CNCLib.Repository.Contract.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.Item", b =>
                {
                    b.HasOne("CNCLib.Repository.Contract.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.ItemProperty", b =>
                {
                    b.HasOne("CNCLib.Repository.Contract.Entities.Item", "Item")
                        .WithMany("ItemProperties")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.Machine", b =>
                {
                    b.HasOne("CNCLib.Repository.Contract.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.MachineCommand", b =>
                {
                    b.HasOne("CNCLib.Repository.Contract.Entities.Machine", "Machine")
                        .WithMany("MachineCommands")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CNCLib.Repository.Contract.Entities.MachineInitCommand", b =>
                {
                    b.HasOne("CNCLib.Repository.Contract.Entities.Machine", "Machine")
                        .WithMany("MachineInitCommands")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

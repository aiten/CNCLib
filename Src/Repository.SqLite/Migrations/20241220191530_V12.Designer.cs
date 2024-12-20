﻿// <auto-generated />
using System;
using CNCLib.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CNCLib.Repository.SqLite.Migrations
{
    [DbContext(typeof(CNCLibContext))]
    [Migration("20241220191530_V12")]
    partial class V12
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ConfigurationEntity", b =>
                {
                    b.Property<int>("ConfigurationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasMaxLength(256)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(256)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.HasKey("ConfigurationId");

                    b.HasIndex("UserId", "Group", "Name")
                        .IsUnique();

                    b.ToTable("Configuration", (string)null);
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ItemEntity", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ItemId");

                    b.HasIndex("UserId", "Name")
                        .IsUnique();

                    b.ToTable("Item", (string)null);
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ItemPropertyEntity", b =>
                {
                    b.Property<int>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("ItemId", "Name");

                    b.ToTable("ItemProperty", (string)null);
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineCommandEntity", b =>
                {
                    b.Property<int>("MachineCommandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("JoystickMessage")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<int>("MachineId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PosX")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PosY")
                        .HasColumnType("INTEGER");

                    b.HasKey("MachineCommandId");

                    b.HasIndex("MachineId");

                    b.ToTable("MachineCommand", (string)null);
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineEntity", b =>
                {
                    b.Property<int>("MachineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Axis")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BaudRate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BufferSize")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ComPort")
                        .IsRequired()
                        .HasMaxLength(32)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<int>("CommandSyntax")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CommandToUpper")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Coolant")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("DtrIsReset")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Laser")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ProbeDist")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ProbeDistUp")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ProbeFeed")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ProbeSizeX")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ProbeSizeY")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ProbeSizeZ")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Rotate")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("SDSupport")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SerialServer")
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("SerialServerPassword")
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("SerialServerUser")
                        .HasMaxLength(32)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SizeA")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SizeB")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SizeC")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SizeX")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SizeY")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SizeZ")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Spindle")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WorkOffsets")
                        .HasColumnType("INTEGER");

                    b.HasKey("MachineId");

                    b.HasIndex("UserId", "Name")
                        .IsUnique();

                    b.ToTable("Machine", (string)null);
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineInitCommandEntity", b =>
                {
                    b.Property<int>("MachineInitCommandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<int>("MachineId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeqNo")
                        .HasColumnType("INTEGER");

                    b.HasKey("MachineInitCommandId");

                    b.HasIndex("MachineId");

                    b.ToTable("MachineInitCommand", (string)null);
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.UserEntity", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasMaxLength(255)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.UserFileEntity", b =>
                {
                    b.Property<int>("UserFileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSystem")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UploadTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserFileId");

                    b.HasIndex("UserId", "FileName")
                        .IsUnique();

                    b.ToTable("UserFile", (string)null);
                });

            modelBuilder.Entity("Framework.Repository.Abstraction.Entities.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Application")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Exception")
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Logger")
                        .HasMaxLength(250)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("MachineName")
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Port")
                        .HasMaxLength(256)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("RemoteAddress")
                        .HasMaxLength(100)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerAddress")
                        .HasMaxLength(100)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerName")
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("StackTrace")
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasMaxLength(500)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasMaxLength(250)
                        .IsUnicode(true)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Log", (string)null);
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ConfigurationEntity", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ItemEntity", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ItemPropertyEntity", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.ItemEntity", "Item")
                        .WithMany("ItemProperties")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineCommandEntity", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.MachineEntity", "Machine")
                        .WithMany("MachineCommands")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Machine");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineEntity", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineInitCommandEntity", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.MachineEntity", "Machine")
                        .WithMany("MachineInitCommands")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Machine");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.UserFileEntity", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ItemEntity", b =>
                {
                    b.Navigation("ItemProperties");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineEntity", b =>
                {
                    b.Navigation("MachineCommands");

                    b.Navigation("MachineInitCommands");
                });
#pragma warning restore 612, 618
        }
    }
}

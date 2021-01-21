﻿// <auto-generated />
using System;
using CNCLib.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CNCLib.Repository.SqlServer.Migrations
{
    [DbContext(typeof(CNCLibContext))]
    partial class CNCLibContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Configuration", b =>
                {
                    b.Property<int>("ConfigurationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.HasKey("ConfigurationId");

                    b.HasIndex("UserId", "Group", "Name")
                        .IsUnique();

                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ItemId");

                    b.HasIndex("UserId", "Name")
                        .IsUnique();

                    b.ToTable("Item");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ItemProperty", b =>
                {
                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ItemId", "Name");

                    b.ToTable("ItemProperty");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Machine", b =>
                {
                    b.Property<int>("MachineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Axis")
                        .HasColumnType("int");

                    b.Property<int>("BaudRate")
                        .HasColumnType("int");

                    b.Property<int>("BufferSize")
                        .HasColumnType("int");

                    b.Property<string>("ComPort")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("CommandSyntax")
                        .HasColumnType("int");

                    b.Property<bool>("CommandToUpper")
                        .HasColumnType("bit");

                    b.Property<bool>("Coolant")
                        .HasColumnType("bit");

                    b.Property<bool>("DtrIsReset")
                        .HasColumnType("bit");

                    b.Property<bool>("Laser")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<decimal>("ProbeDist")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ProbeDistUp")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ProbeFeed")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ProbeSizeX")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ProbeSizeY")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ProbeSizeZ")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("Rotate")
                        .HasColumnType("bit");

                    b.Property<bool>("SDSupport")
                        .HasColumnType("bit");

                    b.Property<string>("SerialServer")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("SerialServerPassword")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("SerialServerUser")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<decimal>("SizeA")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SizeB")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SizeC")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SizeX")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SizeY")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SizeZ")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("Spindle")
                        .HasColumnType("bit");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WorkOffsets")
                        .HasColumnType("int");

                    b.HasKey("MachineId");

                    b.HasIndex("UserId", "Name")
                        .IsUnique();

                    b.ToTable("Machine");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineCommand", b =>
                {
                    b.Property<int>("MachineCommandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("JoystickMessage")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("MachineId")
                        .HasColumnType("int");

                    b.Property<int?>("PosX")
                        .HasColumnType("int");

                    b.Property<int?>("PosY")
                        .HasColumnType("int");

                    b.HasKey("MachineCommandId");

                    b.HasIndex("MachineId");

                    b.ToTable("MachineCommand");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineInitCommand", b =>
                {
                    b.Property<int>("MachineInitCommandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("MachineId")
                        .HasColumnType("int");

                    b.Property<int>("SeqNo")
                        .HasColumnType("int");

                    b.HasKey("MachineInitCommandId");

                    b.HasIndex("MachineId");

                    b.ToTable("MachineInitCommand");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Password")
                        .HasMaxLength(255)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("UserId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.UserFile", b =>
                {
                    b.Property<int>("UserFileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<byte[]>("Content")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<bool>("IsSystem")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UploadTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserFileId");

                    b.HasIndex("UserId", "FileName")
                        .IsUnique();

                    b.ToTable("UserFile");
                });

            modelBuilder.Entity("Framework.Repository.Abstraction.Entities.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Application")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Exception")
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Logger")
                        .HasMaxLength(250)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("MachineName")
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Port")
                        .HasMaxLength(256)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("RemoteAddress")
                        .HasMaxLength(100)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ServerAddress")
                        .HasMaxLength(100)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ServerName")
                        .HasMaxLength(64)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("StackTrace")
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasMaxLength(500)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("UserName")
                        .HasMaxLength(250)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Configuration", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Item", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ItemProperty", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.Item", "Item")
                        .WithMany("ItemProperties")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Machine", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineCommand", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.Machine", "Machine")
                        .WithMany("MachineCommands")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Machine");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineInitCommand", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.Machine", "Machine")
                        .WithMany("MachineInitCommands")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Machine");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.UserFile", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Item", b =>
                {
                    b.Navigation("ItemProperties");
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Machine", b =>
                {
                    b.Navigation("MachineCommands");

                    b.Navigation("MachineInitCommands");
                });
#pragma warning restore 612, 618
        }
    }
}

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
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Configuration", b =>
                {
                    b.Property<int>("ConfigurationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000);

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

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
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Axis")
                        .HasColumnType("int");

                    b.Property<int>("BaudRate")
                        .HasColumnType("int");

                    b.Property<int>("BufferSize")
                        .HasColumnType("int");

                    b.Property<string>("ComPort")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

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
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<bool>("NeedDtr")
                        .HasColumnType("bit");

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
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerialServerPassword")
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<int>("SerialServerPort")
                        .HasColumnType("int");

                    b.Property<string>("SerialServerProtocol")
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("SerialServerUser")
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("JoystickMessage")
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CommandString")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128)
                        .IsUnicode(true);

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255)
                        .IsUnicode(true);

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Content")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(1024)")
                        .HasMaxLength(1024)
                        .IsUnicode(true);

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Application")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(true);

                    b.Property<string>("Exception")
                        .HasColumnType("nvarchar(max)")
                        .IsUnicode(true);

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(true);

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Logger")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250)
                        .IsUnicode(true);

                    b.Property<string>("MachineName")
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64)
                        .IsUnicode(true);

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .IsUnicode(true);

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256)
                        .IsUnicode(true);

                    b.Property<string>("RemoteAddress")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(true);

                    b.Property<string>("ServerAddress")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(true);

                    b.Property<string>("ServerName")
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64)
                        .IsUnicode(true);

                    b.Property<string>("StackTrace")
                        .HasColumnType("nvarchar(max)")
                        .IsUnicode(true);

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500)
                        .IsUnicode(true);

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250)
                        .IsUnicode(true);

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
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Item", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.ItemProperty", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.Item", "Item")
                        .WithMany("ItemProperties")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.Machine", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineCommand", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.Machine", "Machine")
                        .WithMany("MachineCommands")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.MachineInitCommand", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.Machine", "Machine")
                        .WithMany("MachineInitCommands")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CNCLib.Repository.Abstraction.Entities.UserFile", b =>
                {
                    b.HasOne("CNCLib.Repository.Abstraction.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

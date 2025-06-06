﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WindPowerSystemV5.Server.Data;

#nullable disable

namespace WindPowerSystemV5.Server.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250509072210_CreateTurbineTypesAndTurbinesTables")]
    partial class CreateTurbineTypesAndTurbinesTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WindPowerSystemV5.Server.Data.Models.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<decimal>("Lat")
                        .HasColumnType("decimal(7,4)");

                    b.Property<decimal>("Lon")
                        .HasColumnType("decimal(7,4)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("Lat");

                    b.HasIndex("Lon");

                    b.HasIndex("Name");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("WindPowerSystemV5.Server.Data.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ISO2")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ISO3")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ISO2");

                    b.HasIndex("ISO3");

                    b.HasIndex("Name");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("WindPowerSystemV5.Server.Data.Models.Turbine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("TurbineTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TurbineTypeId");

                    b.ToTable("Turbines");
                });

            modelBuilder.Entity("WindPowerSystemV5.Server.Data.Models.TurbineType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("Capacity")
                        .HasColumnType("real");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("TurbineTypes");
                });

            modelBuilder.Entity("WindPowerSystemV5.Server.Data.Models.City", b =>
                {
                    b.HasOne("WindPowerSystemV5.Server.Data.Models.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("WindPowerSystemV5.Server.Data.Models.Turbine", b =>
                {
                    b.HasOne("WindPowerSystemV5.Server.Data.Models.TurbineType", "TurbineType")
                        .WithMany()
                        .HasForeignKey("TurbineTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TurbineType");
                });

            modelBuilder.Entity("WindPowerSystemV5.Server.Data.Models.Country", b =>
                {
                    b.Navigation("Cities");
                });
#pragma warning restore 612, 618
        }
    }
}

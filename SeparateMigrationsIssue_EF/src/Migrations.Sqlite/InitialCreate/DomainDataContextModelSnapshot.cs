﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Migrations.Sqlite.InitialCreate
{
    [DbContext(typeof(DomainDataContext))]
    partial class DomainDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("Domain.Catalog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Catalogs", (string)null);
                });

            modelBuilder.Entity("Domain.CatalogObject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("CatalogId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Constellation")
                        .HasColumnType("TEXT");

                    b.Property<string>("Declination")
                        .HasColumnType("TEXT");

                    b.Property<double>("DistanceLightYears")
                        .HasColumnType("REAL");

                    b.Property<decimal>("Magnitude")
                        .HasColumnType("TEXT");

                    b.Property<string>("MessierNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("NewGeneralCalatog")
                        .HasColumnType("TEXT");

                    b.Property<string>("ObjectType")
                        .HasColumnType("TEXT");

                    b.Property<string>("RightAscension")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.ToTable("Objects", (string)null);
                });

            modelBuilder.Entity("Domain.CatalogObject", b =>
                {
                    b.HasOne("Domain.Catalog", "Catalog")
                        .WithMany("Objects")
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Catalog");
                });

            modelBuilder.Entity("Domain.Catalog", b =>
                {
                    b.Navigation("Objects");
                });
#pragma warning restore 612, 618
        }
    }
}

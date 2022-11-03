﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using tlcn_dotnet;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20221103102426_account_employee_review")]
    partial class account_employee_review
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("tlcn_dotnet.Entity.Account", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("CityId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DetailLocation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DistrictId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("WardId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Bill", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("OrderCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("total")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Bill");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.BillDetail", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<long?>("BillId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long?>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<double>("Quantity")
                        .HasColumnType("float");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("BillId", "ProductId")
                        .IsUnique()
                        .HasFilter("[BillId] IS NOT NULL AND [ProductId] IS NOT NULL");

                    b.ToTable("BillDetail");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Cart", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<long?>("BillId")
                        .HasColumnType("bigint");

                    b.Property<string>("CityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DetailLocation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DistrictId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("WardId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BillId")
                        .IsUnique()
                        .HasFilter("[BillId] IS NOT NULL");

                    b.ToTable("Cart");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.CartDetail", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<long?>("CartId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<double>("Quantity")
                        .HasColumnType("float");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Unit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId", "CartId")
                        .IsUnique()
                        .HasFilter("[ProductId] IS NOT NULL AND [CartId] IS NOT NULL");

                    b.ToTable("CartDetail");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Category", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Employee", b =>
                {
                    b.Property<long?>("Id")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("Salary")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Inventory", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpireDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("ImportPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<double>("Quantity")
                        .HasColumnType("float");

                    b.Property<long?>("SupplierId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("SupplierId");

                    b.ToTable("Inventory");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Product", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<long?>("CategoryId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("MinPurchase")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<double?>("Quantity")
                        .HasColumnType("float");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.ProductImage", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<long?>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImage");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Review", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PostDate")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Rating")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Review");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Supplier", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("CountryCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("detailLocation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Supplier");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.BillDetail", b =>
                {
                    b.HasOne("tlcn_dotnet.Entity.Bill", "Bill")
                        .WithMany("BillDetails")
                        .HasForeignKey("BillId");

                    b.HasOne("tlcn_dotnet.Entity.Product", "Product")
                        .WithMany("BillDetails")
                        .HasForeignKey("ProductId");

                    b.Navigation("Bill");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Cart", b =>
                {
                    b.HasOne("tlcn_dotnet.Entity.Bill", "Bill")
                        .WithOne("Cart")
                        .HasForeignKey("tlcn_dotnet.Entity.Cart", "BillId");

                    b.Navigation("Bill");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.CartDetail", b =>
                {
                    b.HasOne("tlcn_dotnet.Entity.Account", "Account")
                        .WithMany("CartDetails")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tlcn_dotnet.Entity.Cart", "Cart")
                        .WithMany("CartDetails")
                        .HasForeignKey("CartId");

                    b.HasOne("tlcn_dotnet.Entity.Product", "Product")
                        .WithMany("CartDetails")
                        .HasForeignKey("ProductId");

                    b.Navigation("Account");

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Employee", b =>
                {
                    b.HasOne("tlcn_dotnet.Entity.Account", "Account")
                        .WithOne("Employee")
                        .HasForeignKey("tlcn_dotnet.Entity.Employee", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Inventory", b =>
                {
                    b.HasOne("tlcn_dotnet.Entity.Product", "Product")
                        .WithMany("Inventories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("tlcn_dotnet.Entity.Supplier", "Supplier")
                        .WithMany("Inventories")
                        .HasForeignKey("SupplierId");

                    b.Navigation("Product");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Product", b =>
                {
                    b.HasOne("tlcn_dotnet.Entity.Category", "Category")
                        .WithMany("products")
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.ProductImage", b =>
                {
                    b.HasOne("tlcn_dotnet.Entity.Product", "Product")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Review", b =>
                {
                    b.HasOne("tlcn_dotnet.Entity.Account", "Account")
                        .WithMany("Reviews")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Account", b =>
                {
                    b.Navigation("CartDetails");

                    b.Navigation("Employee")
                        .IsRequired();

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Bill", b =>
                {
                    b.Navigation("BillDetails");

                    b.Navigation("Cart")
                        .IsRequired();
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Cart", b =>
                {
                    b.Navigation("CartDetails");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Category", b =>
                {
                    b.Navigation("products");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Product", b =>
                {
                    b.Navigation("BillDetails");

                    b.Navigation("CartDetails");

                    b.Navigation("Inventories");

                    b.Navigation("ProductImages");
                });

            modelBuilder.Entity("tlcn_dotnet.Entity.Supplier", b =>
                {
                    b.Navigation("Inventories");
                });
#pragma warning restore 612, 618
        }
    }
}

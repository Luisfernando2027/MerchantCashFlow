using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ConsolidatedService.Migrations
{
    [DbContext(typeof(ConsolidatedService.Data.ConsolidatedDbContext))]
    partial class Initial_Consolidated
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("ConsolidatedService.Models.Consolidated", b =>
            {
                b.Property<Guid>("Id").ValueGeneratedOnAdd();
                b.Property<Guid>("MerchantId");
                b.Property<DateTime>("Date");
                b.Property<decimal>("Balance");
                b.HasKey("Id");
                b.ToTable("Consolidateds");
            });

            modelBuilder.Entity("ConsolidatedService.Models.ProcessedEvent", b =>
            {
                b.Property<Guid>("EventId");
                b.Property<DateTime>("ProcessedAt");
                b.HasKey("EventId");
                b.ToTable("ProcessedEvents");
            });
        }
    }
}

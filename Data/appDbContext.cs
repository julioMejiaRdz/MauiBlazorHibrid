using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MauiBlazorHibrid.Models;

namespace MauiBlazorHibrid.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Docclientesd> Docclientesds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar clave compuesta si pludocclientes e item forman la PK
            modelBuilder.Entity<Docclientesd>()
                .HasKey(d => new { d.Pludocclientes, d.Item });
        }
    }
}

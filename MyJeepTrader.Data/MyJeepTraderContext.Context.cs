﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyJeepTrader.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dboMyJeepTraderEntities : DbContext
    {
        public dboMyJeepTraderEntities()
            : base("name=dboMyJeepTraderEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tImage> tImages { get; set; }
        public virtual DbSet<tLocation> tLocations { get; set; }
        public virtual DbSet<tMake> tMakes { get; set; }
        public virtual DbSet<tMembership> tMemberships { get; set; }
        public virtual DbSet<tModel> tModels { get; set; }
        public virtual DbSet<tModelYearControl> tModelYearControls { get; set; }
        public virtual DbSet<tPost> tPosts { get; set; }
        public virtual DbSet<tPostType> tPostTypes { get; set; }
        public virtual DbSet<tSubModel> tSubModels { get; set; }
        public virtual DbSet<tUserProfile> tUserProfiles { get; set; }
        public virtual DbSet<tVehicleProfileControl> tVehicleProfileControls { get; set; }
        public virtual DbSet<tVehicleProfile> tVehicleProfiles { get; set; }
        public virtual DbSet<tYear> tYears { get; set; }
        public virtual DbSet<tMessageControl> tMessageControls { get; set; }
        public virtual DbSet<tMessage> tMessages { get; set; }
        public virtual DbSet<tModelPostControl> tModelPostControls { get; set; }
        public virtual DbSet<tConnectedUser> tConnectedUsers { get; set; }
        public virtual DbSet<tState> tStates { get; set; }
        public virtual DbSet<tSubscription> tSubscriptions { get; set; }
    }
}

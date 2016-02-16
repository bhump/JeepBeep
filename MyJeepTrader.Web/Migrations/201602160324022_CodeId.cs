namespace MyJeepTrader.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CodeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CodeId");
        }
    }
}

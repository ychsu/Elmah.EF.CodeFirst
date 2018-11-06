namespace Elmah.EF.CodeFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Elmah_Error",
                c => new
                    {
                        ErrorId = c.Guid(nullable: false, identity: true),
                        Application = c.String(maxLength: 60),
                        Host = c.String(maxLength: 50),
                        Type = c.String(maxLength: 100),
                        Source = c.String(maxLength: 60),
                        Message = c.String(maxLength: 500),
                        User = c.String(maxLength: 50),
                        StatusCode = c.Int(nullable: false),
                        TimeUtc = c.DateTime(nullable: false),
                        Sequence = c.Int(nullable: false, identity: true),
                        AllXml = c.String(),
                    })
                .PrimaryKey(t => t.ErrorId, clustered: false)
                .Index(t => new { t.Application, t.TimeUtc, t.Sequence }, name: "IX_ELMAH_Error_App_Time_Seq", clustered: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Elmah_Error", "IX_ELMAH_Error_App_Time_Seq");
            DropTable("dbo.Elmah_Error");
        }
    }
}

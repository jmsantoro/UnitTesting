namespace DataAccess
{
    using System.Data.Entity;

    public class EcuDbContext : DbContext
    {
        // Your context has been configured to use a 'EcuDbContext' connection string from your application's
        // configuration file (App.config or Web.config). By default, this connection string targets the
        // 'DataAccess.EcuDbContext' database on your LocalDb instance.
        //
        // If you wish to target a different database and/or database provider, modify the 'EcuDbContext'
        // connection string in the application configuration file.
        public EcuDbContext()
            : base("name=EcuDbContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<EcuData> EcuData { get; set; }
    }
}
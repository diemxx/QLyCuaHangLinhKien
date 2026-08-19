using System.Configuration;

namespace ComputerStore_WPF.DataAccess
{
    public static class DatabaseConfig
    {

        public static string GetConnectionString()
        {

            return @"Data Source=Nguyendiem;Initial Catalog=ComputerStoreDB;User ID=sa;Password=123;TrustServerCertificate=True;";
        }
    }
}
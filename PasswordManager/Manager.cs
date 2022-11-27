using Microsoft.Data.Sqlite;
using System.Windows.Controls;

namespace PasswordManager
{
    public static class Manager
    {
        public static string GetConnectionString() => SqLiteConnectionString.ToString();

        public static SqliteConnectionStringBuilder SqLiteConnectionString { get; set; } = null!;

        public static Frame MainFrame { get; set; } = null!;

    }
}

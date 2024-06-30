using System.Data.SQLite;

namespace DBConnector.Accessor
{
    public interface IDbAccessor
    {
        /// <summary>
        /// 接続先文字列
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// ConnectionStringに設定された接続先へのConnectionオブジェクトを取得する
        /// </summary>
        public SQLiteConnection Connection { get; }
    }

    public class MonetaryManagementDataAdapter : IDbAccessor
    {

        public MonetaryManagementDataAdapter()
        {
        }

        /// <inheritdoc />
        public string ConnectionString => Properties.Settings.Default.ConnectionString;

        /// <inheritdoc />
        public SQLiteConnection Connection => new SQLiteConnection(ConnectionString);
    }
}

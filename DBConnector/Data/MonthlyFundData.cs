using DBConnector.Accessor;
using DBConnector.Extention;
using System.Text;

namespace DBConnector.Data
{
    /// <summary>
    /// 月初総額情報テーブル【テーブル名：MonthlFund】回りデータ操作インターフェース
    /// </summary>
    public interface IMonthlyFundData
    {
        /// <summary>
        /// MonthlyFundテーブルの作成を行う
        /// </summary>
        public void CreateTable();

        /// <summary>
        /// MonthlyFundが存在するか否かチェックする
        /// </summary>
        /// <returns></returns>
        public bool IsExistMonthlyFundTable();

        /// <summary>
        /// 指定した年月の最新残高が存在するか否か
        /// </summary>
        /// <param name="year">取得したい年</param>
        /// <param name="month">取得したい月</param>
        /// <returns></returns>
        public bool IsExistMonthFirstBalance(int year, int month);

        /// <summary>
        /// 指定した年月の最新残高を取得する
        /// </summary>
        /// <param name="year">取得したい年</param>
        /// <param name="month">取得したい月</param>
        /// <returns></returns>
        public decimal LoadMonthFirstBalance(int year, int month);

        /// <summary>
        /// 最新月の月初残額を取得する
        /// </summary>
        /// <returns></returns>
        public decimal LoadRecentMonthFirstPrice();

        /// <summary>
        /// MonthlyFundに引数で指定した年月の初日の残額情報を挿入する
        /// </summary>
        /// <param name="year">指定する年月（西暦四ケタ）</param>
        /// <param name="month">指定する月</param>
        public void InsertMonthlyFundRecord(int year, int month, decimal firstBalance);
    }

    public class MonthlyFundData : IMonthlyFundData
    {
        /// <summary>
        /// DB接続オブジェクト
        /// </summary>
        private readonly IDbAccessor _db;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="db"></param>
        public MonthlyFundData(IDbAccessor db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public bool IsExistMonthlyFundTable()
        {
            var query = $"SELECT COUNT(*) FROM sqlite_master WHERE TYPE='table' AND name='MonthlyFund'";
            int resultNum = (int)_db.Connection.ExecuteQueryWithValue<long>(query);
            return resultNum > 0;
        }

        /// <inheritdoc />
        public void CreateTable()
        {
            var query = new StringBuilder();
            query.AppendLine("CREATE TABLE \"MonthlyFund\" (");
            query.AppendLine("\t\"ID\"\tINTEGER NOT NULL UNIQUE,");
            query.AppendLine("\t\"Year\"\tINTEGER NOT NULL,");
            query.AppendLine("\t\"Month\"\tINTEGER NOT NULL,");
            query.AppendLine("\t\"Price\"\tdecimal(28, 0),");
            query.AppendLine("\tPRIMARY KEY(\"ID\" AUTOINCREMENT)");
            query.AppendLine(")");
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public decimal LoadMonthFirstBalance(int year, int month)
        {
            var query = new StringBuilder();
            query.AppendLine("SELECT [Price] ");
            query.AppendLine($"FROM [MonthlyFund] ");
            query.AppendLine($"WHERE [Year] = {year} ");
            query.AppendLine($"AND[Month] = {month} ");
            query.AppendLine($"ORDER BY [MonthlyFund].ID DESC LIMIT 1");
            return _db.Connection.ExecuteQueryWithValue<decimal>(query.ToString());
        }

        /// <inheritdoc />
        public bool IsExistMonthFirstBalance(int year, int month)
        {
            var query = new StringBuilder();
            query.AppendLine("SELECT COUNT([Price]) ");
            query.AppendLine($"FROM [MonthlyFund] ");
            query.AppendLine($"WHERE [Year] = {year} ");
            query.AppendLine($"AND[Month] = {month} ");
            query.AppendLine($"ORDER BY [MonthlyFund].ID DESC LIMIT 1");
            return _db.Connection.ExecuteQueryWithValue<long>(query.ToString()) > 0;
        }

        /// <inheritdoc />
        public decimal LoadRecentMonthFirstPrice()
        {
            var query = $" SELECT [Price] FROM[MonthlyFund] ORDER BY[MonthlyFund].ID DESC LIMIT 1";
            return _db.Connection.ExecuteQueryWithValue<decimal>(query);
        }

        /// <inheritdoc />
        public void InsertMonthlyFundRecord(int year, int month, decimal firstBalance)
        {
            var query = $"INSERT INTO [MonthlyFund]([Year],[Month],[Price]) VALUES ({year},{month},{firstBalance})";
            _db.Connection.ApplyChangeDataTable(query);
        }

    }
}

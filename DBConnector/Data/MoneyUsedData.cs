using DBConnector.Accessor;
using DBConnector.Entity;
using DBConnector.Extention;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DBConnector.Data
{
    /// <summary>
    /// 月別利用金額テーブル【テーブル名：yyyy(年４桁)-mm（月２桁）】 回り データ操作 インターフェース
    /// </summary>
    public interface IMoneyUsedData
    {
        /// <summary>
        /// DataTableから取得した金銭管理データリスト
        /// </summary>
        public IEnumerable<MoneyUsedDataEntity> LoadMoneyUsedData(string year, string month);

        /// <summary>
        /// 引数で受け取った金銭管理データをデータベースにアップロードする
        /// </summary>
        /// <param name="moneyUsedData">アップロードしたい金銭管理データ</param>
        public void UploadMonetaryData(IEnumerable<MoneyUsedDataEntity> moneyUsedData, string year, string month);

        /// <summary>
        /// 金銭管理データの消去※
        /// </summary>
        public void DeleteMonetaryData(string year, string month);

        /// <summary>
        /// 金銭管理テーブル新規作成
        /// </summary>
        /// <param name="year">存在確認をしたい年</param>
        /// <param name="month">存在確認をしたい月</param>
        public void CreateTable(string year, string month);

        /// <summary>
        /// 月単位の合計利用金額を取得する。
        /// </summary>
        /// <param name="year">取得したい年</param>
        /// <param name="month">取得したい月(2桁)</param>
        /// <returns></returns>
        public decimal LoadMonthlySumPrice(string year, string month);

        /// <summary>
        /// テーブルが存在するか確認する
        /// </summary>
        /// <param name="year">存在確認をしたい年</param>
        /// <param name="month">存在確認をしたい月</param>
        /// <returns></returns>
        public bool IsExistMonetaryTable(string year, string month);

        /// <summary>
        /// 月別利用額テーブル名を降順で一括取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> MonthlyTableNames();
    }

    /// <summary>
    /// 月別利用金額テーブル【テーブル名：yyyy(年４桁)-mm（月２桁）】 回り データ操作 クラス
    /// </summary>
    public class MoneyUsedData : IMoneyUsedData
    {
        /// <summary>
        /// DB接続オブジェクト
        /// </summary>
        private readonly IDbAccessor _db;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="db"></param>
        public MoneyUsedData(IDbAccessor db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public void CreateTable(string year, string month)
        {
            var query = $"CREATE TABLE [{year}-{month}] (" +
                                     "ID integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                                     "[Date] date," +
                                     "Price decimal(28, 0)," +
                                     "Classification char(1)" +
                                     ")";
            _db.Connection.ApplyChangeDataTable(query);
        }

        /// <inheritdoc />
        public void DeleteMonetaryData(string year, string month)
        {
            var query = $"DELETE FROM [{year}-{month}]; DELETE FROM sqlite_sequence WHERE name = {year}-{month}";
            _db.Connection.ApplyChangeDataTable(query);
        }

        /// <inheritdoc />
        public decimal LoadMonthlySumPrice(string year, string month)
        {
            var query = $"SELECT CAST(SUM([Price]) AS decimal(28, 0)) FROM [{year}-{month}]";
            return _db.Connection.ExecuteQueryWithValue<decimal>(query);
        }

        /// <inheritdoc />
        public bool IsExistMonetaryTable(string year, string month)
        {
            var query = $"SELECT COUNT(*) FROM sqlite_master WHERE TYPE='table' AND name='{year}-{month}'";
            int resultNum = (int)_db.Connection.ExecuteQueryWithValue<long>(query);
            return resultNum > 0;
        }

        /// <inheritdoc />
        public IEnumerable<MoneyUsedDataEntity> LoadMoneyUsedData(string year, string month)
        {
            var query = $"SELECT * FROM [{year}-{month}]";
            return _db.Connection.GetData<MoneyUsedDataEntity>(query);
        }

        /// <inheritdoc />
        public IEnumerable<string> MonthlyTableNames()
        {
            var query = "SELECT name FROM sqlite_master WHERE type='table' AND name GLOB '[0-9][0-9][0-9][0-9]-[0-9][0-9]'";
            return _db.Connection.GetData<string>(query).OrderByDescending(name => name).ToArray();
        }

        /// <inheritdoc />
        public void UploadMonetaryData(IEnumerable<MoneyUsedDataEntity> moneyUsedData, string year, string month)
        {
            var query = new StringBuilder($"INSERT INTO [{year}-{month}] VALUES ");
            query.AppendLine(string.Join(",", moneyUsedData.Select(item => $"({item.ID},'{item.Date.Replace('/', '-')}',{item.Price},'{item.Classification}')").ToArray()));
            query.AppendLine("ON CONFLICT(ID) ");
            query.AppendLine("DO UPDATE ");
            query.AppendLine("SET [Date] = excluded.Date, [Price] = excluded.Price, [Classification] = excluded.Classification; ");
            query.AppendLine($"DELETE FROM [{year}-{month}] ");
            query.AppendLine($"WHERE ID NOT IN ({string.Join(",", moneyUsedData.Select(x => x.ID).ToArray())})");

            _db.Connection.ApplyChangeDataTable(query.ToString());
        }
    }
}

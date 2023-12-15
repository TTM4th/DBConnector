using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using DBConnector.Entity;
using System.Data.SQLite;

namespace DBConnector.Accessor
{
    public class MoneyUsedDataAccessor
    {

        /// <summary>
        /// 接続用Connectionオブジェクト
        /// </summary>
        private SQLiteConnection Connection;

        /// <summary>
        /// 接続したいテーブル名
        /// </summary>
        private string TableName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tableName">接続したいテーブル名</param>
        public MoneyUsedDataAccessor(string tableName){TableName = tableName;}

        /// <summary>
        /// DataTableから取得した金銭管理データリスト
        /// </summary>
        public IReadOnlyList<MoneyUsedData> MoneyUsedDataEntitiesFromTable { get; private set; }

        /// <summary>
        /// テーブルから金銭管理データを取得する。
        /// </summary>
        public void GetMonetarydata()
        {
            Connection=new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            Connection.Open();
            var adapter = new SQLiteDataAdapter { SelectCommand = new SQLiteCommand($"SELECT * FROM [{TableName}]", Connection) };
            var dataSet=new DataSet();
            adapter.Fill(dataSet);
            MoneyUsedDataEntitiesFromTable = dataSet.Tables.OfType<DataTable>().Single()
                                        .Rows.OfType<DataRow>().Select(row => new MoneyUsedData(row)).ToList();
            Connection.Close();
        }


        /// <summary>
        /// 引数で受け取った金銭管理データをデータベースにアップロードする(一括挿入)
        /// </summary>
        /// <param name="moneyUsedData">アップロードしたい金銭管理データ</param>
        public void UploadMonetaryData(IEnumerable<MoneyUsedData> moneyUsedData)
        {
            Connection = new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            

            using (SQLiteCommand command = Connection.CreateCommand())
            {
                SQLiteTransaction transaction = null;
                try
                {
                    Connection.Open();
                    transaction = Connection.BeginTransaction();
                    command.CommandText = BuildReplaceQuery(moneyUsedData);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        /// <summary>
        /// 引数で受け取った金銭管理データをDBに反映させるためのクエリを発行する
        /// </summary>
        /// <param name="data">変換したい金銭管理データ</param>
        /// <returns></returns>
        internal string BuildReplaceQuery(IEnumerable<MoneyUsedData> data)
        {
            string query = $"REPLACE INTO [{TableName}] VALUES ";
            //列とデータ型を設定したテーブルにデータを入れる
            foreach (MoneyUsedData item in data)
            {
                query += $"({item.ID},'{item.Date.Replace('/','-')}',{item.Price},'{item.Classification}'),";
            }
            return query.TrimEnd(',');//最後の余分なカンマは消して返す。
        }

        ///// <summary>
        ///// 月単位の合計利用金額を取得する。
        ///// ※引数で指定した年月の月別テーブルが無い場合は引数で指定した年月分の空のテーブルを作成してから月単位の合計金額を取得する。
        ///// </summary>
        ///// <param name="year">取得したい年</param>
        ///// <param name="month">取得したい月</param>
        ///// <returns></returns>
        //public static decimal GetMonthlyPrice(int year,int month)
        //{
        //    using (var monthlyUsedManager = new MoneyUsedDataTableManager()) { 
        //        //指定月の月別利用額テーブルが存在しない場合は作成する。
        //        if (monthlyUsedManager.IsExistMonetaryTable($"{year}-{month.ToString("00")}") == false) { monthlyUsedManager.CreateTable($"{year}-{month.ToString("00")}"); }
        //    }
        //
        //    decimal gotBalance;
        //
        //    using (var connection = new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString })
        //    {
        //        connection.Open();
        //        var command = new SQLiteCommand(connection);
        //        command.CommandText = $"SELECT SUM([Price]) FROM [{year}-{month.ToString("00")}]";
        //        command.ExecuteNonQuery();
        //        gotBalance = DBNull.Value.Equals(command.ExecuteScalar()) ? 0 : Convert.ToDecimal(command.ExecuteScalar());
        //        connection.Close();
        //    }
        //
        //    return gotBalance;
        //}

    }

}

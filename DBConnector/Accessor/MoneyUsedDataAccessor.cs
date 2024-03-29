﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using DBConnector.Entity;
using System.Data.SQLite;
using System.Text;

namespace DBConnector.Accessor
{
    public class MoneyUsedDataAccessor
    {
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
            var connection=new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            connection.Open();
            var adapter = new SQLiteDataAdapter { SelectCommand = new SQLiteCommand($"SELECT * FROM [{TableName}]", connection) };
            var dataSet=new DataSet();
            adapter.Fill(dataSet);
            MoneyUsedDataEntitiesFromTable = dataSet.Tables.OfType<DataTable>().Single()
                                        .Rows.OfType<DataRow>().Select(row => new MoneyUsedData(row)).ToList();
            connection.Close();
        }


        /// <summary>
        /// 引数で受け取った金銭管理データをデータベースにアップロードする
        /// </summary>
        /// <param name="moneyUsedData">アップロードしたい金銭管理データ</param>
        public void UploadMonetaryData(IEnumerable<MoneyUsedData> moneyUsedData)
        {
            this.UpdateDataTable(BuildReplaceQuery(moneyUsedData));
        }

        /// <summary>
        /// 金銭管理データの消去※
        /// </summary>
        public void DeleteMonetaryData()
        {
            var query = $"DELETE FROM [{TableName}]; DELETE FROM sqlite_sequence WHERE name = {TableName}";
            this.UpdateDataTable(query);
        }

        private void UpdateDataTable(string commandText)
        {
            var connection = new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            using (SQLiteCommand command = connection.CreateCommand())
            {
                SQLiteTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    command.CommandText = commandText;
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
                    connection.Close();
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
            var query = new StringBuilder($"INSERT INTO [{TableName}] VALUES ");
            var values = data.Select(item => $"({item.ID},'{item.Date.Replace('/', '-')}',{item.Price},'{item.Classification}')").ToList();
            query.Append(string.Join(",", values));
            query.Append("ON CONFLICT(ID) ");
            query.Append("DO UPDATE ");
            query.Append("SET [Date] = excluded.Date, [Price] = excluded.Price, [Classification] = excluded.Classification; ");
            query.Append($"DELETE FROM [{TableName}] ");
            var ids = string.Join(",", data.Select(x => x.ID).ToList());
            query.Append($"WHERE ID NOT IN ({ids})");
            return query.ToString();
        }

    }

}

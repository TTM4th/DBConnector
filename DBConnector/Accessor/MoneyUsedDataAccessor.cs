using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using DBConnector.Entity;

namespace DBConnector.Accessor
{
    public class MoneyUsedDataAccessor
    {

        /// <summary>
        /// 接続用Connectionオブジェクト
        /// </summary>
        private SqlConnection Connection;

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
            Connection=new SqlConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            Connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter { SelectCommand = new SqlCommand($"SELECT * FROM [dbo].[{TableName}]", Connection) };
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
            Connection = new SqlConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            Connection.Open();
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(Connection))
            {
                bulkCopy.BulkCopyTimeout = 600;
                bulkCopy.DestinationTableName = $"[{TableName}]";
                bulkCopy.WriteToServer(ToDataTable(moneyUsedData));
            }
            Connection.Close();
        }

        /// <summary>
        /// 引数で受け取った金銭管理データをDataTable形式にする
        /// </summary>
        /// <param name="data">変換したい金銭管理データ</param>
        /// <returns></returns>
        internal static DataTable ToDataTable(IEnumerable<MoneyUsedData> data)
        {
            //var properties = TypeDescriptor.GetProperties(typeof(MoneyUsedData));
            var table = new DataTable();
            //項目変数プロパティに従った列とデータ型を設定する
            table.Columns.Add(nameof(MoneyUsedData.ID), typeof(int));
            table.Columns.Add(nameof(MoneyUsedData.Date), typeof(DateTime));
            table.Columns.Add(nameof(MoneyUsedData.Price), typeof(decimal));
            table.Columns.Add(nameof(MoneyUsedData.Classification), typeof(string));
            //foreach (PropertyDescriptor prop in properties)
            //{
            //    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            //}
            //列とデータ型を設定したテーブルにデータを入れる
            foreach (MoneyUsedData item in data)
            {
                var row = table.NewRow();
                //foreach (PropertyDescriptor prop in properties){row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;}
                row[nameof(MoneyUsedData.ID)] = item.ID;
                row[nameof(MoneyUsedData.Date)] = DateTime.Parse(item.Date);
                row[nameof(MoneyUsedData.Price)] = item.Price;
                row[nameof(MoneyUsedData.Classification)] = item.Classification;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// 月単位の合計利用金額を取得する
        /// </summary>
        /// <param name="year">取得したい年</param>
        /// <param name="month">取得したい月</param>
        /// <returns></returns>
        public static decimal GetMonthlyPrice(int year,int month)
        {
            string getMonthlySumProcedureName ="ReturnSumMonthlyPrice";
            string outputParamName = "@Result";
            string storedParamName1="@targetYear";
            string storedParamName2 ="@targetMonth";

            SqlConnection connection = new SqlConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            connection.Open();
            SqlCommand command = new SqlCommand(getMonthlySumProcedureName, connection){ CommandType = CommandType.StoredProcedure};
            command.Parameters.AddWithValue(storedParamName1, year);
            command.Parameters.AddWithValue(storedParamName2, month);
            command.Parameters.Add(outputParamName, SqlDbType.Decimal);
            command.Parameters[outputParamName].Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();
            decimal gotBalance = DBNull.Value.Equals(command.Parameters[outputParamName].Value) ? 0 : (decimal)command.Parameters[outputParamName].Value;
            connection.Close();
            return gotBalance;
        }

    }

}

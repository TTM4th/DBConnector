using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public MoneyUsedDataAccessor(string tableName)
        {
            Connection = new SqlConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            TableName = tableName;
        }

        /// <summary>
        /// DataTableから取得した金銭管理データリスト
        /// </summary>
        public IReadOnlyList<MoneyUsedData> MoneyUsedDataEntitiesFromTable { get; private set; }

        /// <summary>
        /// テーブルから金銭管理データを取得する。
        /// </summary>
        public void GetMonetarydata()
        {
            Connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter { SelectCommand = new SqlCommand($"SELECT * FROM [dbo].[{TableName}]", Connection) };
            var dataSet=new DataSet();
            adapter.Fill(dataSet);
            MoneyUsedDataEntitiesFromTable = dataSet.Tables.OfType<DataTable>().Single()
                                        .Rows.OfType<DataRow>().Select(row => new MoneyUsedData(row)).ToList();
            Connection.Close();
        }


        //undone:更新ロジック
    }

}

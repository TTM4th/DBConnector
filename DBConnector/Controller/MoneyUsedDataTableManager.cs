using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace DBConnector.Controller
{
    public class MoneyUsedDataTableManager
    {

        /// <summary>
        /// 接続用Connectionオブジェクト
        /// </summary>
        private SqlConnection Connection;

        /// <summary>
        /// テーブル存在有無フラグ列挙体
        /// </summary>
        private enum IsExistTable{NotExist,Exist}


        /// <summary>
        /// 引数で渡した処理をConnection接続→引数で渡した処理→接続解除の順で行うクロージャ
        /// </summary>
        protected Action<Action> ConnectionClosure;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoneyUsedDataTableManager()
        {
            ConnectionClosure = (Action action) => {
                Connection = new SqlConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
                Connection.Open(); action(); Connection.Close();
            };
        }


        /// <summary>
        /// テーブルが存在するか確認する
        /// </summary>
        /// <param name="tableName">存在確認をしたいテーブル名</param>
        /// <returns></returns>
        public bool IsExistMonetaryTable(string tableName)
        {
            bool isExist=false;
            ConnectionClosure( () =>
                {
                    SqlCommand SelectCommand = new SqlCommand($"IF OBJECT_ID(N'[{tableName}]', N'U') IS NULL SELECT {(int)IsExistTable.NotExist} ELSE SELECT {(int)IsExistTable.Exist}", Connection);
                    isExist = (IsExistTable)SelectCommand.ExecuteScalar() == IsExistTable.Exist;
                }
            );
            return isExist;
        }

        /// <summary>
        /// テーブルを作成する
        /// </summary>
        /// <param name="tableName"></param>
        public void CreateTable(string tableName)
        {
            ConnectionClosure(() => 
                {
                    SqlCommand command = new SqlCommand(Properties.Settings.Default.CreateTableSProcedure, Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@TableName", tableName));
                    command.ExecuteNonQuery();
                }
            );
        }

       
        /// <summary>
        /// テーブルを初期化する
        /// </summary>
        /// <param name="tableName"></param>
       public void InitializeTable(string tableName)
       {
            if (IsExistMonetaryTable(tableName) == false) { return; }
            ConnectionClosure(() =>
                {
                    SqlCommand command = new SqlCommand($"TRUNCATE TABLE [dbo].[{tableName}] ", Connection);
                    command.ExecuteNonQuery();
                }
            );
       }

        /// <summary>
        /// 月別利用額テーブル名を降順で一括取得する
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> MonthlyTableNames()
        {
            DataTable dataTable = new DataTable();
            const int firstColumnIndex = 0;
            ConnectionClosure( () =>
                {
                    SqlCommand command = new SqlCommand(Properties.Settings.Default.GetAllTableNames, Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    adapter.Dispose();
                    command.Dispose();
                }
            );
            return dataTable.Rows.OfType<DataRow>().Select(x=>x.Field<string>(firstColumnIndex)).Where(x => x.Contains("-")).OrderByDescending(name => name).ToList();
        }

    }
}

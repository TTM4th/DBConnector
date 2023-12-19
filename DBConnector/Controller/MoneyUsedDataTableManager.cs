using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;

namespace DBConnector.Controller
{
    public class MoneyUsedDataTableManager : IDisposable
    {

        /// <summary>
        /// 接続用Connectionオブジェクト
        /// </summary>
        private SQLiteConnection Connection;


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
                Connection = new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
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
            var isExisttablecount = 0;
            ConnectionClosure(() =>
               {
                   SQLiteCommand SelectCommand = new SQLiteCommand($"SELECT COUNT(*) FROM sqlite_master WHERE TYPE='table' AND name='{tableName}'", Connection);
                   isExisttablecount = Convert.ToInt32(SelectCommand.ExecuteScalar());
               }
            );
            if (isExisttablecount != 0) return true;
            else return false;
        }

        /// <summary>
        /// テーブルを作成する
        /// </summary>
        /// <param name="tableName"></param>
        public void CreateTable(string tableName)
        {
            ConnectionClosure(() =>
                {
                    SQLiteCommand command = new SQLiteCommand(Connection)
                    {
                        CommandText = $"CREATE TABLE [{tableName}] ("+
                                     "ID integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                                     "[Date] date,"+
                                     "Price decimal(28, 0),"+
                                     "Classification char(1)"+
                                     ")"
                    };
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
                    SQLiteCommand command = new SQLiteCommand($"DELETE FROM [{tableName}] ", Connection);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand($"VACUUM", Connection);
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
            ConnectionClosure(() =>
               {
                   SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name GLOB '[0-9][0-9][0-9][0-9]-[0-9][0-9]'", Connection);
                   SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                   adapter.Fill(dataTable);
                   adapter.Dispose();
                   command.Dispose();
               }
            );
            return dataTable.Rows.OfType<DataRow>().Select(x => x.Field<string>(firstColumnIndex)).OrderByDescending(name => name).ToList();
        }

        /// <summary>
        /// 最新年月のテーブルの月別利用総額を取得する
        /// </summary>
        /// <returns>最新年月テーブルの月別利用総額</returns>
        public decimal LastMonthlySumPrice()
        {
            var yeardate = this.MonthlyTableNames().First<string>().Split('-');
            return this.GetMonthlyPrice(yeardate[0], yeardate[1]);
        }

        /// <summary>
        /// 月単位の合計利用金額を取得する。
        /// </summary>
        /// <param name="year">取得したい年</param>
        /// <param name="month">取得したい月(2桁)</param>
        /// <returns></returns>
        public decimal GetMonthlyPrice(string year, string month)
        {

            decimal gotBalance = 0;
            ConnectionClosure(() =>
            {
                SQLiteCommand command = new SQLiteCommand(Connection);
                command.CommandText = $"SELECT SUM([Price]) FROM [{year}-{month}]";
                var result = command.ExecuteScalar();
                gotBalance = DBNull.Value.Equals(result) ? 0 : Convert.ToDecimal(result);
            });

            return gotBalance;
        }


        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                    Connection.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~MoneyUsedDataTableManager() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}

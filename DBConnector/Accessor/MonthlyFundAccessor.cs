using System;
using System.Data.SqlClient;

namespace DBConnector.Accessor
{
    /// <summary>
    /// 月別月初残高テーブルのAcceserクラス
    /// </summary>
    public class MonthlyFundAccessor
    {

        #region 各種変数・プロパティ
        /// <summary>
        /// 接続用Connectionオブジェクト
        /// </summary>
        private SqlConnection Connection;

        /// <summary>
        /// 月別月初残高テーブル名
        /// </summary>
        private string TableName { get { return "MonthlyFund"; } }

        /// <summary>
        /// 月初残額取得ストアドプロシージャ名
        /// </summary>
        private string GetBalanceProcedureName { get { return "GetMonthFirstBalance"; } }

        /// <summary>
        /// GetMonthFirstBalanceプロシージャの第3引数名(OUTPUTパラメータ)
        /// </summary>
        private string OutputParamName { get { return "@Result"; } }

        /// <summary>
        /// GetMonthFirstBalanceプロシージャの第1引数名
        /// </summary>
        private string StoredParamName1 { get { return "@targetYear"; } }

        /// <summary>
        /// GetMonthFirstBalanceプロシージャの第2引数名
        /// </summary>
        private string StoredParamName2 { get { return "@targetMonth"; } }

        # endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MonthlyFundAccessor() { }

        /// <summary>
        /// 指定した年月の最新残高を取得する
        /// </summary>
        /// <param name="year">取得したい年</param>
        /// <param name="month">取得したい月</param>
        /// <returns></returns>
        public decimal GetMonthFirstBalance(int year,int month)
        {
                Connection = new SqlConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
                Connection.Open();
                SqlCommand command = new SqlCommand(GetBalanceProcedureName,Connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue(StoredParamName1, year);
                command.Parameters.AddWithValue(StoredParamName2, month);
                command.Parameters.Add(OutputParamName, System.Data.SqlDbType.Decimal);
                command.Parameters[OutputParamName].Direction = System.Data.ParameterDirection.Output;
                command.ExecuteNonQuery();
                decimal gotBalance = Convert.ToDecimal(command.Parameters[OutputParamName].Value);
                Connection.Close();
                return gotBalance;
        }

    }
}

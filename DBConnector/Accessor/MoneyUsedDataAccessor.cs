using DBConnector.Data;
using DBConnector.Entity;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace DBConnector.Accessor
{
    public class MoneyUsedDataAccessor
    {
        /// <summary>
        /// 接続したいテーブル名
        /// </summary>
        private string TableName;

        private readonly IMoneyUsedData _data;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tableName">接続したいテーブル名</param>
        public MoneyUsedDataAccessor(string tableName, IMoneyUsedData data)
        {
            TableName = tableName;
            _data = data;
        }

        /// <summary>
        /// テーブルから金銭管理データを取得する。
        /// </summary>
        public void GetMonetarydata()
        {
            _data.LoadMoneyUsedData(TableName);
        }

        public void UploadMonetaryData(List<MoneyUsedDataEntity> uploadObj)
        {
            _data.UploadMonetaryData(uploadObj, TableName);
        }
    }

}

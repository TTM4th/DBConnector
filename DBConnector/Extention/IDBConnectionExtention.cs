using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;

namespace DBConnector.Extention
{
    /// <summary>
    /// SQLiteDBConnectionの拡張メソッド
    /// </summary>
    public static class SQLiteDBConnectionExtention
    {

        /// <summary>
        /// データテーブルの更新ロジック
        /// </summary>
        /// <param name="query">更新内容クエリ</param>
        public static void ApplyChangeDataTable(this SQLiteConnection connection, string query)
        {
            var command = connection.CreateCommand();
            SQLiteTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                command.CommandText = query;
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

        /// <summary>
        /// クエリを実行し、実行結果を単一の値で返す
        /// </summary>
        /// <param name="connection">マップしたいクラス情報</param>
        /// <param name="query">クエリ</param>
        /// <returns></returns>
        public static outType ExecuteQueryWithValue<outType>(this SQLiteConnection connection, string query)
        {
            outType returnObj;
            using (connection)
            {
                var selectCommand = new SQLiteCommand(query, connection);
                connection.Open();
                returnObj = (outType)selectCommand.ExecuteScalar();
            }
            return returnObj;
        }

        /// <summary>
        /// データ取得・オブジェクトマッピング（DapperでいうQueryメソッド）
        /// </summary>
        /// <typeparam name="outType">マップしたいクラス情報</typeparam>
        /// <param name="query">クエリ</param>
        /// <returns></returns>
        public static IEnumerable<outType> GetData<outType>(this SQLiteConnection connection, string query)
        {
            return connection.GetResult(query, MapFactory<outType>()).ToArray();
        }

        /// <summary>
        /// クエリ結果の１件目のデータ取得・オブジェクトマッピングする
        /// </summary>
        /// <typeparam name="outType">マップしてほしい型</typeparam>
        /// <param name="query">クエリ</param>
        /// <returns></returns>
        public static outType GetFirstOrDefaultData<outType>(this SQLiteConnection connection, string query)
        {
            return connection.GetResult(query, MapFactory<outType>()).FirstOrDefault();
        }

        /// <summary>
        /// 引数で渡したクエリを基にマップされたオブジェクトの列挙子を返す（遅延実行用）
        /// </summary>
        /// <typeparam name="outType">マップしてほしい型</typeparam>
        /// <param name="connection"></param>
        /// <param name="query">クエリ</param>
        /// <param name="predMap">マップしてほしい型にマップする匿名関数</param>
        /// <returns></returns>
        public static IEnumerable<outType> GetResult<outType>(this SQLiteConnection connection, 
                                                            string query, 
                                                            Func<IDataRecord, outType> predMap)
        {
            IEnumerable<outType> result = Enumerable.Empty<outType>();
            using (connection)
            {
                var selectCommand = new SQLiteCommand(query, connection);
                connection.Open();
                result = selectCommand.EnumerateAll().Select(row => predMap(row));
            }
            return result;
        }

        /// <summary>
        /// 型情報からクラス用・構造体マッパか値マッパを返す
        /// </summary>
        /// <typeparam name="outType"></typeparam>
        /// <returns></returns>
        private static Func<IDataRecord, outType> MapFactory<outType>()
        {
            var type = typeof(outType);
            if (type.IsValueType || type == typeof(string))
            {
                return MappingValue<outType>;
            }
            return Mapping<outType>;
        }

        /// <summary>
        /// マッピング処理 構造体・クラス用
        /// </summary>
        /// <param name="row">取得したデータテーブルの行</param>
        /// <returns></returns>
        private static outType Mapping<outType>(IDataRecord row)
        {
            var outObjType = typeof(outType);
            var outObj = (outType)Activator.CreateInstance(outObjType);
            foreach (var fieldName in outObjType.GetFields().Select(x => x.Name))
            {
                outObjType.GetField(fieldName, BindingFlags.Public).SetValue(outObj, row[fieldName]);
            }
            return outObj;
        }

        /// <summary>
        /// マッピング処理 値用
        /// </summary>
        /// <typeparam name="outType"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        private static outType MappingValue<outType>(IDataRecord row)
        {
            outType outObj = (outType)row[0];
            return outObj;
        }
    }

}

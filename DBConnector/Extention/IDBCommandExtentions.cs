using System.Collections.Generic;
using System.Data;

namespace DBConnector.Extention
{
    /// <summary>
    /// IDbCommandの拡張メソッド
    /// </summary>
    public static class IDBCommandExtentions
    {
        /// <summary>
        /// DbCommandで取得した結果をIDataRecord型のイテレータで取得する
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static IEnumerable<IDataRecord> EnumerateAll(this IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read()) yield return reader;
            }
        }
    }
}

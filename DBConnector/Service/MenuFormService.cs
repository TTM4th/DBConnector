using DBConnector.Data;
using DBConnector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Appointments;

namespace DBConnector.Service
{
    public class MenuFormService
    {
        /// <summary>
        /// 月初総額情報テーブル【テーブル名：MonthlFund】回り データ操作
        /// </summary>
        private readonly IMonthlyFundData _monthlyFund;

        /// <summary>
        /// 月別利用金額テーブル【テーブル名：yyyy(年４桁)-mm（月２桁）】 回り データ操作
        /// </summary>
        private readonly IMoneyUsedData _moneyUsed;

        private int NowYear => DateTime.Now.Year;

        private int NowMonth => DateTime.Now.Month;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monthlyFund"></param>
        /// <param name="moneyUsed"></param>
        public MenuFormService(IMonthlyFundData monthlyFund, IMoneyUsedData moneyUsed)
        {
            _monthlyFund = monthlyFund;
            _moneyUsed = moneyUsed;
        }

        /// <summary>
        /// 月別総額情報テーブルの存在有無をチェックする
        /// </summary>
        /// <returns></returns>
        public bool IsExistMonthlyFundTable()
        {
            return _monthlyFund.IsExistMonthlyFundTable();
        }

        /// <summary>
        /// 月別総額情報テーブルを生成する
        /// </summary>
        public void CreateMonthlyFundTable()
        {
            _monthlyFund.CreateTable();
        }

        /// <summary>
        /// 当月の月始残高の存在有無を確認する
        /// </summary>
        /// <returns></returns>
        public bool IsExistFirstBalance()
        {
            return _monthlyFund.IsExistMonthFirstBalance(NowYear, NowMonth);
        }

        /// <summary>
        /// 前月の利用金額総額と前月の月初金額から当月の月始金額を計算して月初金額テーブル(MonthlyFund)に挿入する
        /// </summary>
        public void InsertFromPreviousMonth()
        {
            var recentInitialbalance = _monthlyFund.LoadRecentMonthFirstPrice();
            var yeardate = _moneyUsed.MonthlyTableNames().First().Split('-');
            var recentMonthSumBalance = _moneyUsed.LoadMonthlySumPrice(yeardate[0], yeardate[1]);
            _monthlyFund.InsertMonthlyFundRecord(NowYear, NowMonth, (recentInitialbalance - recentMonthSumBalance));
        }

        /// <summary>
        /// 当月の月別金額テーブル【テーブル名：yyyy(年４桁)-mm（月２桁）】が存在するか
        /// </summary>
        /// <returns></returns>
        public bool IsExistMonetaryTable()
        {
            return _moneyUsed.IsExistMonetaryTable(NowYear.ToString(), NowMonth.ToString("00"));
        }

        /// <summary>
        /// 当月の月別金額テーブル【テーブル名：yyyy(年４桁)-mm（月２桁）】を作成する
        /// </summary>
        /// <returns></returns>
        public void CreateMonetaryTable()
        {
            _moneyUsed.CreateTable(NowYear.ToString(), NowMonth.ToString("00"));
        }

        /// <summary>
        /// 引数で指定した年月の区分別集計データを取得する
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        public IDictionary<string, decimal> LoadSumPricesByClassification(string year, string month)
        {
            return _moneyUsed.LoadMoneyUsedData(year, month).GroupBy(x => x.Classification).ToDictionary(x => x.Key, x => x.Select(x => x.Price).Sum());
        }

        public decimal GetCurrentBalance()
        {
            return _monthlyFund.LoadMonthFirstBalance(NowYear, NowMonth) - _moneyUsed.LoadMonthlySumPrice(NowYear.ToString(), NowMonth.ToString("00"));
        }

        /// <summary>
        /// ビュー用モデルを取得する
        /// </summary>
        /// <returns></returns>
        public MenuFormModel CreateViewModel()
        {
            return new MenuFormModel
            {
                MonthlyTableNames = _moneyUsed.MonthlyTableNames().Take(6).ToArray(),
                CurrentBalance = GetCurrentBalance(),
                MoneyUsedData = _moneyUsed.LoadMoneyUsedData(NowYear.ToString(), NowMonth.ToString("00")).GroupBy(x => x.Classification).ToDictionary(x => x.Key, x => x.Select(x => x.Price).Sum())
            };
        }

    }
}

﻿using DBConnector.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBConnector.Service
{
    public class MenuFormService
    {
        private readonly IMonthlyFundData _monthlyFund;

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
        /// 当月の月始残高の存在有無を確認する
        /// </summary>
        /// <returns></returns>
        public bool IsExistFirstBalance()
        {
            return _monthlyFund.LoadMonthFirstBalance(NowYear, NowMonth) != null;
        }

        /// <summary>
        /// 前月の利用金額総額と前月の月初金額から当月の月始金額を計算して月初金額テーブル(MonthlyFund)に挿入する
        /// </summary>
        public void InsertFromPreviousMonth()
        {
            var recentInitialbalance = _monthlyFund.LoadRecentMonthFirstPrice();
            var yeardate = _moneyUsed.MonthlyTableNames().First().Split('-');
            var recentMonthSumBalance = _moneyUsed.LoadMonthlySumPrice(yeardate[0], yeardate[1]);
            _monthlyFund.InsertMonthlyFundRecord(NowYear, NowMonth, (decimal)recentInitialbalance - recentMonthSumBalance);
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

        public MenuFormModel CreateViewModel()
        {
            return new MenuFormModel
            {
                MonthlyTableNames = _moneyUsed.MonthlyTableNames().Take(6).ToArray(),
                CurrentBalance = (decimal)_monthlyFund.LoadMonthFirstBalance(NowYear, NowMonth) - _moneyUsed.LoadMonthlySumPrice(NowYear.ToString(), NowMonth.ToString("00"))
            };
        }

    }

    public class MenuFormModel
    {
        public IEnumerable<string> MonthlyTableNames { get; set; }
        public decimal CurrentBalance { get; set;}
    }
}
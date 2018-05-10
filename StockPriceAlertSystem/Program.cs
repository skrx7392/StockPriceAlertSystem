using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MessageBoxer;
using Newtonsoft.Json;

namespace StockAlertSystem
{
  class Program
  {
    static HttpClient client = new HttpClient();
    static void Main(string[] args)
    {
      var symbols = InsertSymbols();
      client.BaseAddress = new Uri("https://api.robinhood.com/quotes/");
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

      RunStockTickers(symbols);
    }
    private static async Task RunStockTickers(List<StockTicker> symbols)
    {
      while (DateTime.Now.TimeOfDay <=
             new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0).TimeOfDay)
      {
        var stockTickers = new List<StockTicker>();
        foreach (var symbol in symbols)
        {
          ReadStocks(symbol);
        }
        Thread.Sleep(30000);
      }
    }

    private static List<StockTicker> InsertSymbols()
    {
      return new List<StockTicker>()
      {
        new StockTicker()
        {
          symbol = "NFLX",
          last_trade_price = 330
        },
        new StockTicker()
        {
          symbol = "ALGN",
          last_trade_price = 280
        },
        new StockTicker()
        {
          symbol = "CLDR",
          last_trade_price = 16.48m
        }
      };
    }

    private static async void ReadStocks(StockTicker symbol)
    {
      try
      {
        StockTicker stockTicker = null;
        HttpResponseMessage response = await client.GetAsync($"{ symbol.symbol}/");
        if (response.IsSuccessStatusCode)
        {
          var data = await response.Content.ReadAsStringAsync();
          stockTicker = JsonConvert.DeserializeObject<StockTicker>(data);
          if (stockTicker.last_trade_price >= symbol.last_trade_price)
          {
            ShowMessage.Notification(symbol.symbol + " has exceeded the target price with current price " + stockTicker.last_trade_price);
          }
        }

      }
      catch (Exception ex)
      {
        ShowMessage.Error(ex.Message);
      }
    }
  }
  public class StockTicker
  {
    public decimal last_trade_price { get; set; }
    public string symbol { get; set; }
  }

}

using System;

namespace DaprSample.Shared.Models
{
    public class AuctionGoods
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long StartPrice { get; set; }
        public long NowPrice { get; set; }
        public long MaxPriceUserId { get; set; }
        public bool RePut { get; set; }
    }
}

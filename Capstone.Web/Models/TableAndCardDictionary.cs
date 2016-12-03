using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class TableAndCardDictionary
    {
        public Table Table { get; set; }
        public Dictionary<Card, bool> CardList { get; set; }
        //possibly add something for other players' cards.
    }
}
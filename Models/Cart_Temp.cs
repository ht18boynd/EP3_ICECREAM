using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP3_ICE_CREAM.Models
{
    public class Cart_Temp
    {
        // Process when Customer choose product in Cart to buy
        public string _book_id { get; set; }
        public string _book_name { get; set; }
        public string _image_main { get; set; }
        public int _price { get; set; }
        public int _quantity { get; set; }
        public int _sum
        {
            get
            {
                return _price * _quantity;
            }
        }
        public bool _check { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WOAPI
{
    public class clsMO
    {
        public String MO_Number { get; set; }
        public int MO_Status { get; set; }
        public String Item_Num { get; set; }
        public String Item_Desc { get; set; }
        public decimal Total_Quantity { get; set; }
        public DateTime Start_Date { get; set; }
    }

    public class clsMIL
    {
        public DateTime TrasDate { get; set; }
        public String RefNo { get; set; }
        public int Account { get; set; }
        public Decimal Debit { get; set; }
        public Decimal Credit { get; set; }
    }

    public class clsResponse
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
    }
    public class ReverseAPIResponse
    {
        public string mO_NUMBER { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClientApp.General
{
    public class CommonMethods
    {
        private static DateTime financialStartDate = new DateTime(2000,4,1);
        private static DateTime financialEndDate = new DateTime(2000, 3, 31);

        public static DateTime FinancialStartDate
        {
            get
            {
                return financialStartDate;
            }
            set
            {
                financialStartDate = value;
            }
        }

        public static DateTime FinancialEndDate
        {
            get
            {
                return financialEndDate;
            }
            set
            {
                financialEndDate = value;
            }
        }


        public static string getFinancialCode(DateTime dateTime)
        {
            string financialCode = "2015";
            if (dateTime.Month >= FinancialStartDate.Month)
            {
                if (dateTime.Day>=FinancialStartDate.Day)
                {
                    financialCode = dateTime.Year.ToString();
                }
                else
                {
                    financialCode = (dateTime.Year - 1).ToString();
                }
            }
            else
            {
                financialCode = (dateTime.Year - 1).ToString();
            }

            return financialCode;
        }

        public static DateTime getFinancialStartDate(string financialCode)
        {
            DateTime startDate = new DateTime();
            try {
                int fYear = int.Parse(financialCode);
                startDate = new DateTime(fYear,FinancialStartDate.Month, FinancialStartDate.Day);
            }
            catch
            {

            }
            
            return startDate;
        }

        public static DateTime getFinancialEndDate(string financialCode)
        {
            DateTime endDate = new DateTime();
            try
            {
                int fYear = int.Parse(financialCode)+1;                
                endDate = new DateTime(fYear, FinancialEndDate.Month, FinancialEndDate.Day);
            }
            catch
            {

            }
            return endDate;
        }
    }
}

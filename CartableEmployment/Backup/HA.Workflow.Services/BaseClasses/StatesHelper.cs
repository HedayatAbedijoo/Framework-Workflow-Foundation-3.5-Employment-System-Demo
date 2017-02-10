using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace HA.Workflow.Services.BaseClasses
{
    public class StatesHelper
    {
        public static string GetPageMapping(string Key)
        {
            throw new Exception();
        }

        public static string GetPageNext(string key)
        {
            throw new Exception();

        }

        public static string GetPreviousNext(string key)
        {
            throw new Exception();

        }

        public static DataSet GetEventsName(string Key)
        {
            throw new Exception();

        }

        public static string GetGotoPage(string Key)
        {
            return "";// DataLayer.GetData.GetPageToGoTo(Key);
        }

        public static int GetGotoPageId(string Key)
        {
            return 1;//DataLayer.GetData.GetPageToGoToId(Key);
        }
        public static string GetStartPage()
        {
            return "";// DataLayer.GetData.GetStarterPage();
        }
    }
}

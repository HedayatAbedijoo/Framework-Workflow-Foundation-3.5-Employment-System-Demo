using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HA.Workflow.Services.Interfaces;
using System.Collections;

namespace HA.Workflow.Services.Services
{
    public class CustomConditionsService : ICustomConditions
    {

        #region ICustomConditions Members

        public bool GoToConfirmApplicant(StateBehaveArgs item)
        {
            ArrayList arr = item.Parameters[0] as ArrayList;
            long score = 0;
            for (int i = 0; i < arr.Count; i++)
            {
                try
                {
                    score += !string.IsNullOrEmpty(arr[i].ToString()) ? Convert.ToInt64(arr[i]) : 0;
                }
                catch (Exception)
                {


                }
            }

            if (score > 100)
                return true;
            else
                return false;
        }

        public bool InterviewUnSucceed(StateBehaveArgs item)
        {
            ArrayList arr = item.Parameters[0] as ArrayList;
            long score = 0;
            for (int i = 0; i < arr.Count; i++)
            {
                try
                {
                    score += !string.IsNullOrEmpty(arr[i].ToString()) ? Convert.ToInt64(arr[i]) : 0;
                }
                catch (Exception)
                {


                }
            }

            if (score < 100)
                return true;
            else
                return false;
        }

        public bool InterviewSucceed(StateBehaveArgs item)
        {
            ArrayList arr = item.Parameters[0] as ArrayList;
            long score = 0;
            for (int i = 0; i < arr.Count; i++)
            {
                try
                {
                    score += !string.IsNullOrEmpty(arr[i].ToString()) ? Convert.ToInt64(arr[i]) : 0;
                }
                catch (Exception)
                {


                }
            }

            if (score > 100)
                return true;
            else
                return false;
        }

        #endregion

    }
}

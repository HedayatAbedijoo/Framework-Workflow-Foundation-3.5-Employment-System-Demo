using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.Workflow.Services.Interfaces
{
    public interface ICustomConditions
    {
        bool GoToConfirmApplicant(StateBehaveArgs item);
        bool InterviewUnSucceed(StateBehaveArgs item);
        bool InterviewSucceed(StateBehaveArgs item);

    }
}

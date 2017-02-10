using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design;

namespace HA.Workflow.Services.BaseClasses
{
    public class StateEventsWaiting
    {
        [Serializable]
        public class EventInfo
        {
            #region Private Variables
            private string eventName;
            private string eventAlias;
            private DateTime dateOfHire;
            #endregion

            #region Public Properties
            [Category("Event")]
            [DisplayName("Event Name")]
            [Description("The name of the event that state will waited to raise.")]
            public string EventName
            {
                get { return eventName; }
                set { eventName = value; }
            }

            [Category("Event")]
            [DisplayName("Event Alias")]
            [Description("The alias name of the event.")]
            public string EventAlias
            {
                get { return eventAlias; }
                set { eventAlias = value; }
            }

            #endregion
        }

        [Serializable]
        public class EventInfoCollection : CollectionBase
        {
            public EventInfo this[int index]
            {
                get { return (EventInfo)List[index]; }
            }

            public void Add(EventInfo emp)
            {
                List.Add(emp);
            }

            public void Remove(EventInfo emp)
            {
                List.Remove(emp);
            }
        }

        [Serializable]
        public class EventInfoCollectionEditor : CollectionEditor
        {
            public EventInfoCollectionEditor(Type type)
                : base(type)
            {
            }

            protected override string GetDisplayText(object value)
            {
                EventInfo item = new EventInfo();
                item = (EventInfo)value;

                return base.GetDisplayText(string.Format("{0} : {1}", item.EventAlias, item.EventName));
            }
        }

    }
}

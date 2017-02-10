using System;
using System.Collections;
using System.Transactions;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;

namespace WFTools.Services.Tracking
{
    /// <summary>
    /// Abstract implementation of <see cref="TrackingChannel" /> that provides the
    /// framework necessary for a very generic tracking service. 
    /// 
    /// A resource provider and accessor work hand-in-hand to actually
    /// persist workflow tracking information to the persistence store.
    /// </summary>
    public abstract class GenericWorkflowTrackingChannel : TrackingChannel, IPendingWork
    {
        protected GenericWorkflowTrackingChannel(IResourceProvider resourceProvider)
        {
            if (resourceProvider == null)
                throw new ArgumentNullException("resourceProvider");

            this.resourceProvider = resourceProvider;
        }

        /// <summary>
        /// The active <see cref="IResourceProvider" /> that provides 
        /// resources necessary for manipulating the underlying tracking store.
        /// </summary>
        private IResourceProvider resourceProvider;

        /// <summary>
        /// Create an <see cref="IResourceAccessor" /> that is responsible
        /// for manipulating the underlying tracking store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        protected abstract IResourceAccessor CreateAccessor(IResourceProvider resourceProvider);

        /// <summary>
        /// Create an <see cref="IResourceAccessor" /> that is responsible
        /// for manipulating the underlying tracking store.
        /// </summary>
        /// <param name="resourceProvider">
        /// The active <see cref="IResourceProvider" />.
        /// </param>
        /// <param name="transaction">
        /// Transaction to perform operations within.
        /// </param>
        protected abstract IResourceAccessor CreateAccessor(IResourceProvider resourceProvider, Transaction transaction);

        ///<summary>
        ///When implemented in a derived class, sends a <see cref="T:System.Workflow.Runtime.Tracking.TrackingRecord"></see> on the <see cref="T:System.Workflow.Runtime.Tracking.TrackingChannel"></see>.
        ///</summary>
        ///
        ///<param name="record">The <see cref="T:System.Workflow.Runtime.Tracking.TrackingRecord"></see> to send.</param>
        protected override void Send(TrackingRecord record)
        {
            WorkflowEnvironment.WorkBatch.Add(this, record);
        }

        ///<summary>
        ///When implemented in a derived class, notifies a receiver of data on the tracking channel that the workflow instance associated with the tracking channel has either completed or terminated.
        ///</summary>
        ///
        protected override void InstanceCompletedOrTerminated()
        {
        }

        ///<summary>
        ///Allows the items in the work batch to assert whether they must commit immediately.
        ///</summary>
        ///
        ///<returns>
        ///true if any item in the collection must be committed immediately; otherwise false.
        ///</returns>
        ///
        ///<param name="items">An <see cref="T:System.Collections.ICollection"></see> of work items.</param>
        public bool MustCommit(ICollection items)
        {
            return false;
        }

        ///<summary>
        ///Commits the list of work items by using the specified <see cref="T:System.Transactions.Transaction"></see> object.
        ///</summary>
        ///
        ///<param name="items">The work items to be committed.</param>
        ///<param name="transaction">The <see cref="T:System.Transactions.Transaction"></see> associated with the pending work.</param>
        public void Commit(Transaction transaction, ICollection items)
        {
            // iterate all the activities and persist them to the database
            using (IResourceAccessor resourceAccessor = CreateAccessor(resourceProvider, transaction))
            {
                //TODO - attempt to
                //  - Insert workflow
                //  - Insert workflow instance
                //
                // This will give us back an 'internal' id

                foreach (object itemToCommit in items)
                {
                    if (!(itemToCommit is TrackingRecord))
                        continue;
                    else if (itemToCommit is ActivityTrackingRecord)
                        ;
                    else if (itemToCommit is UserTrackingRecord)
                        ;
                    else if (itemToCommit is WorkflowTrackingRecord)
                        ;
                }
            }
        }

        ///<summary>
        ///Called when the transaction has completed.
        ///</summary>
        ///
        ///<param name="items">An <see cref="T:System.Collections.ICollection"></see> of work items.</param>
        ///<param name="succeeded">true if the transaction succeeded; otherwise, false.</param>
        public void Complete(bool succeeded, ICollection items)
        {
        }
    }
}

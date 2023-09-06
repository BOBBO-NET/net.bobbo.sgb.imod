using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BobboNet.SGB.IMod
{
    /// <summary>
    /// An event collection that functions via each action returning an approval result.
    /// </summary>
    /// <typeparam name="T">The type of the args passed to each action</typeparam>
    public class ApproveEvent<T>
    {
        //
        //  Types
        //

        public delegate ApproveEventResult EventDelegate(T args);

        //
        //  Private Variables
        //

        private List<EventDelegate> actions = new List<EventDelegate>();

        //
        //  Methods
        //

        /// <summary>
        /// Add an action that should be considered as a source of approval, when getting approval later.
        /// </summary>
        /// <param name="approvalSource"></param>
        public void AddApprovalSource(EventDelegate approvalSource) => actions.Add(approvalSource);

        /// <summary>
        /// Remove an action that was previously a source of approval.
        /// </summary>
        /// <param name="approvalSource"></param>
        /// <returns>true if the given source was removed, false if it was not even found.</returns>
        public bool RemoveApprovalSource(EventDelegate approvalSource) => actions.Remove(approvalSource);


        public ApproveEventResult GetApproval(T args)
        {
            // Clone the action list to prevent some weirdness
            var currentActions = new List<EventDelegate>(actions);

            // Go through each action, and if one does NOT approve, then EXIT EARLY.
            foreach (EventDelegate action in currentActions)
            {
                ApproveEventResult result = action.Invoke(args);
                if (result != ApproveEventResult.Approve) return result;
            }

            // We only get here if all actions approve this event.
            return ApproveEventResult.Approve;
        }
    }
}
// <copyright file="PathChangedEvent.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    /// <summary>
    /// Event for publishing and subscribing to a change in the Source URI for an application.
    /// </summary>
    public class PathChangedEvent : PubSubEvent<string>
    {
    }
}

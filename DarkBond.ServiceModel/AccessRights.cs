// <copyright file="AccessRights.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    using System;

    /// <summary>
    /// Access rights for an object.
    /// </summary>
    [Flags]
    public enum AccessRights
    {
        /// <summary>
        /// The right to browse a folder.
        /// </summary>
        Browse = 0x0001,

        /// <summary>
        /// The right to execute code.
        /// </summary>
        Execute = 0x0002,

        /// <summary>
        /// The right to execute code and browse.
        /// </summary>
        ExecuteBrowse = 0x0003,

        /// <summary>
        /// Full control over the object.
        /// </summary>
        FullControl = 0x000F,

        /// <summary>
        /// No Rights.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// The right to read an object.
        /// </summary>
        Read = 0x0004,

        /// <summary>
        /// The right to read and object and browse.
        /// </summary>
        ReadBrowse = 0x0005,

        /// <summary>
        /// The right to read an object and execute code.
        /// </summary>
        ReadExecute = 0x0006,

        /// <summary>
        /// The right to read an object, execute code or browse a folder.
        /// </summary>
        ReadExecuteBrowse = 0x0007,

        /// <summary>
        /// The right to read or write an object.
        /// </summary>
        ReadWrite = 0x000C,

        /// <summary>
        /// The right to read or write and object or browse a folder.
        /// </summary>
        ReadWriteBrowse = 0x000D,

        /// <summary>
        /// The right to read or write an object or execute code.
        /// </summary>
        ReadWriteExecute = 0x000E,

        /// <summary>
        /// The right to write an object.
        /// </summary>
        Write = 0x0008,

        /// <summary>
        /// The right to write an object and browse a folder.
        /// </summary>
        WriteBrowse = 0x0009,

        /// <summary>
        /// The right to write an object and execute code.
        /// </summary>
        WriteExecute = 0x000A,

        /// <summary>
        /// The right to write an object, execute code or browse a folder.
        /// </summary>
        WriteExecuteBrowse = 0x000B
    }
}

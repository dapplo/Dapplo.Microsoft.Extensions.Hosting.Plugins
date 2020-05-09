// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro
{
    /// <summary>
    /// The Caliburn context contains all information for Caliburn
    /// </summary>
    public interface ICaliburnMicroContext
    {
        /// <summary>
        /// This make it possible to pass the data-context of the originally clicked object in the Message.Attach event bubbling.
        /// E.G. the parent Menu-Item Click will get the Child MenuItem that was actually clicked.
        /// </summary>
        bool EnableOriginalDataContext { get; set; }
    }
}
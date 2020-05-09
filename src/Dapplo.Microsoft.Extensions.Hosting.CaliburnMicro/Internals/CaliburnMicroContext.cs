// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro.Internals
{
    /// <inheritdoc />
    public class CaliburnMicroContext : ICaliburnMicroContext
    {
        /// <summary>
        /// Enable a way to get the original data context, the value for this is $originalDataContext
        /// </summary>
        public bool EnableOriginalDataContext { get; set; }
    }
}
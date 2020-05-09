// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins
{
    /// <summary>
    ///     Use this attribute to specify the order for loading plug-ins
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PluginOrderAttribute : Attribute
    {
        /// <summary>
        /// Default value
        /// </summary>
        public PluginOrderAttribute()
        {
        }

        /// <summary>
        /// Specify the order of the plug-in initialization
        /// </summary>
        public PluginOrderAttribute(int order)
        {
            Order = order;
        }

        /// <summary>
        /// Specify the order by using enums
        /// </summary>
        /// <param name="enumValue">object which can be converted to an int</param>
        public PluginOrderAttribute(object enumValue) : this(Convert.ToInt32(enumValue))
        {
        }

        /// <summary>
        /// Order to initialize the plug-in
        /// </summary>
        public int Order { get; }
    }
}

// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace Dapplo.Hosting.Sample.WinFormsDemo
{
    public partial class Form2 : Form
    {
        private readonly ILogger<Form2> _logger;

        public Form2(ILogger<Form2> logger)
        {
            _logger = logger;
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            _logger.LogDebug("Closing form2");
            Close();
        }
    }
}

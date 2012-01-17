// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Security.Permissions;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.WAPPSCmdlet;

namespace AzureDeploymentCmdlets.Cmdlet
{
    /// <summary>
    /// Save a new deployment package (.cspack) for the current Windows Azure service. Overwrites the existing .cspack.
    /// </summary>
    [Cmdlet(VerbsData.Save, "AzureServicePackage")]
    public class SaveAzureServicePackageCommand : ServiceManagementCmdletBase
    {
        private AzureService _azureService;

        /// <summary>
        /// Initializes a new instance of the SaveAzureServicePackageCommand class.
        /// </summary>
        public SaveAzureServicePackageCommand(){}

        /// <summary>
        /// Execute the command
        /// </summary>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                SafeWriteObjectWithTimestamp(Resources.PackageAzureServiceStartMessage);
                CreatePackage(GetServiceRootPath());
                SafeWriteObjectWithTimestamp(Resources.PackageAzureServiceFinishedMessage);

            }
            catch(Exception ex)
            {
                SafeWriteError(ex);
            }
        }

        /// <summary>
        /// Create a new .cspack for the Azure path hosted at the specified root path
        /// </summary>
        /// <param name="rootPath">Root path of the Azure service.</param>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        internal void CreatePackage(string rootPath)
        {
            Debug.Assert(!string.IsNullOrEmpty(rootPath), "rootPath cannot be null or empty.");
            Debug.Assert(Directory.Exists(rootPath), "rootPath does not exist.");

            _azureService = new AzureService(rootPath, null);

            string unused = null;
            _azureService.CreatePackage(DevEnv.Cloud, out unused, out unused);
        }
    }
}

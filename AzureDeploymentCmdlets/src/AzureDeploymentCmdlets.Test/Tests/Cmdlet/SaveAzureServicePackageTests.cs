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
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using AzureDeploymentCmdlets.Cmdlet;
using AzureDeploymentCmdlets.Node.Cmdlet;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Test.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    /// <summary>
    /// Tests for the Save-AzureServicePackage command.
    /// </summary>
    [TestClass]
    public class SaveAzureServicePackageTests : TestBase
    {
        /// <summary>
        /// Test a basic packaging scenario for an Azure Service Package with a single web role.
        /// </summary>
        [TestMethod]
        public void SavePackageWithOneNodeWebRoleTest()
        {
            //Create a temp directory for monitoring and cleaning up the output of our test
            using(FileSystemHelper files = new FileSystemHelper(this){EnableMonitoring = true})
            {
                //Create a new service that we're going to pack locally
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);

                //Add a Node web role to the solution
                string roleName = "TEST_WEB_ROLE";
                int instanceCount = 2;
                AddAzureNodeWebRoleCommand addAzureNodeWebRole = new AddAzureNodeWebRoleCommand();
                addAzureNodeWebRole.AddAzureNodeWebRoleProcess(roleName, instanceCount, servicePath);

                //Run our packaging command
                SaveAzureServicePackageCommand saveServicePackage = new SaveAzureServicePackageCommand();
                saveServicePackage.CreatePackage(servicePath);

                //Assert that the service structure is as expected
                AzureAssert.ScaffoldingExists(Path.Combine(files.RootPath, serviceName, roleName), Path.Combine(Resources.NodeScaffolding, Resources.WebRole));

                // Verify the generated files
                files.AssertFiles(new Dictionary<string, Action<string>>()
                {
                    {
                        serviceName + @"\deploymentSettings.json",
                        null
                    },
                    {
                        serviceName + @"\ServiceDefinition.csdef",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\ServiceConfiguration.Cloud.cscfg",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\ServiceConfiguration.Local.cscfg",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\cloud_package.cspkg",
                        p =>
                        {
                            using (Package package = Package.Open(p))
                            {
                                Assert.AreEqual(6, package.GetParts().Count());
                            }
                        }
                    }
                });

            }
        }

        /// <summary>
        /// Test a basic packaging scenario for an Azure Service Package with a single web role.
        /// </summary>
        [TestMethod]
        public void SavePackageWithOneNodeWorkerRoleTest()
        {
            //Create a temp directory for monitoring and cleaning up the output of our test
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                //Create a new service that we're going to pack locally
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);

                //Add a Node web role to the solution
                string roleName = "TEST_WORKER_ROLE";
                int instanceCount = 2;
                AddAzureNodeWorkerRoleCommand addAzureNodeWorkerRole = new AddAzureNodeWorkerRoleCommand();
                addAzureNodeWorkerRole.AddAzureNodeWorkerRoleProcess(roleName, instanceCount, servicePath);

                //Run our packaging command
                SaveAzureServicePackageCommand saveServicePackage = new SaveAzureServicePackageCommand();
                saveServicePackage.CreatePackage(servicePath);

                //Assert that the service structure is as expected
                AzureAssert.ScaffoldingExists(Path.Combine(files.RootPath, serviceName, roleName), Path.Combine(Resources.NodeScaffolding, Resources.WorkerRole));

                // Verify the generated files
                files.AssertFiles(new Dictionary<string, Action<string>>()
                {
                    {
                        serviceName + @"\deploymentSettings.json",
                        null
                    },
                    {
                        serviceName + @"\ServiceDefinition.csdef",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\ServiceConfiguration.Cloud.cscfg",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\ServiceConfiguration.Local.cscfg",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\cloud_package.cspkg",
                        p =>
                        {
                            using (Package package = Package.Open(p))
                            {
                                Assert.AreEqual(6, package.GetParts().Count());
                            }
                        }
                    }
                });

            }
        }

        /// <summary>
        /// Test a basic packaging scenario for an Azure Service Package with a single web role.
        /// </summary>
        [TestMethod]
        public void SavePackageWithMultipleRolesTest()
        {
            //Create a temp directory for monitoring and cleaning up the output of our test
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                //Create a new service that we're going to pack locally
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);

                //Add a Node web role to the solution
                string webRoleName = "TEST_WEB_ROLE";
                int webRoleInstanceCount = 2;
                AddAzureNodeWebRoleCommand addAzureNodeWebRole = new AddAzureNodeWebRoleCommand();
                addAzureNodeWebRole.AddAzureNodeWebRoleProcess(webRoleName, webRoleInstanceCount, servicePath);

                //Add a Node web role to the solution
                string workerRoleName = "TEST_WORKER_ROLE";
                int workerRoleInstanceCount = 2;
                AddAzureNodeWorkerRoleCommand addAzureNodeWorkerRole = new AddAzureNodeWorkerRoleCommand();
                addAzureNodeWorkerRole.AddAzureNodeWorkerRoleProcess(workerRoleName, workerRoleInstanceCount, servicePath);

                //Run our packaging command
                SaveAzureServicePackageCommand saveServicePackage = new SaveAzureServicePackageCommand();
                saveServicePackage.CreatePackage(servicePath);

                //Assert that the service structure is as expected
                AzureAssert.ScaffoldingExists(Path.Combine(files.RootPath, serviceName, workerRoleName), Path.Combine(Resources.NodeScaffolding, Resources.WorkerRole));
                AzureAssert.ScaffoldingExists(Path.Combine(files.RootPath, serviceName, webRoleName), Path.Combine(Resources.NodeScaffolding, Resources.WebRole));

                // Verify the generated files
                files.AssertFiles(new Dictionary<string, Action<string>>()
                {
                    {
                        serviceName + @"\deploymentSettings.json",
                        null
                    },
                    {
                        serviceName + @"\ServiceDefinition.csdef",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\ServiceConfiguration.Cloud.cscfg",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\ServiceConfiguration.Local.cscfg",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\cloud_package.cspkg",
                        p =>
                        {
                            using (Package package = Package.Open(p))
                            {
                                Assert.AreEqual(7, package.GetParts().Count());
                            }
                        }
                    }
                });

            }
        }
    }
}

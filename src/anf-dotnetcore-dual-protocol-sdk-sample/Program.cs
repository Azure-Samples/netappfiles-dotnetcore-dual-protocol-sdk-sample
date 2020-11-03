// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

namespace Microsoft.Azure.Management.ANF.Samples
{
    using Microsoft.Azure.Management.ANF.Samples.Common;
    using Microsoft.Azure.Management.NetApp;
    using Microsoft.Azure.Management.NetApp.Models;
    using Microsoft.Rest;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using static Microsoft.Azure.Management.ANF.Samples.Common.Utils;

    class Program
    {
        //------------------------------------------IMPORTANT------------------------------------------------------------------
        // Setting variables necessary for resources creation - change these to appropriated values related to your environment
        // Please NOTE: Resource Group and VNETs need to be created prior to run this code
        //----------------------------------------------------------------------------------------------------------------------

        // Subscription - Change SubId below
        const string subscriptionId = "<Subscription ID>";

        const string resourceGroupName = "<Resource Group Name>";
        const string location = "westus";
        const string subnetId = "<Subnet ID>";
        const string anfAccountName = "anftestaccount";
        const string capacityPoolName = "anfprimarypool";
        const string capacityPoolServiceLevel = "Standard";
        const long capacitypoolSize = 4398046511104;  // 4TiB which is minimum size
        const string anfVolumeName = "anftestvolume";
        const long volumeSize = 107374182400;  // 100GiB - volume minimum size    

        const string domainJoinUsername = "<Domain Join Username>";
        const string domainJoinPassword = "<Domain Join Password>";
        const string dnsList = "10.0.0.4,10.0.0.5"; // please note that this is a comma-seperated string
        const string adFQDN = "testdomain.local";
        const string smbServerNamePrefix = "pmcdns";
        const string rootCACertFullFilePath = "<Root Certification Full File Path>";

        // If resources should be cleaned up
        static readonly bool shouldCleanUp = false;

        private static ServiceClientCredentials Credentials { get; set; }

        /// <summary>
        /// Sample console application that changes a volume from one Capacity Pool to another in different service level tier
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DisplayConsoleAppHeader();
            try
            {
                CreateANFAsync().GetAwaiter().GetResult();
                WriteConsoleMessage("Sample application successfuly completed execution.");
            }
            catch (Exception ex)
            {
                WriteErrorMessage(ex.Message);
            }
        }

        private static async Task CreateANFAsync()
        {
            //----------------------------------------------------------------------------------------
            // Authenticating using service principal, refer to README.md file for requirement details
            //----------------------------------------------------------------------------------------
            WriteConsoleMessage("Authenticating...");
            Credentials = await ServicePrincipalAuth.GetServicePrincipalCredential("AZURE_AUTH_LOCATION");

            //------------------------------------------
            // Instantiating a new ANF management client
            //------------------------------------------
            WriteConsoleMessage("Instantiating a new Azure NetApp Files management client...");
            AzureNetAppFilesManagementClient anfClient = new AzureNetAppFilesManagementClient(Credentials)
            {
                SubscriptionId = subscriptionId
            };
            WriteConsoleMessage($"\tApi Version: {anfClient.ApiVersion}");


            //----------------------------
            // Get Certification encoding
            //----------------------------            
            var bytes = Encoding.UTF8.GetBytes(Utils.GetFileContent(rootCACertFullFilePath));
            var encodedCertContent = Convert.ToBase64String(bytes);

            //----------------------
            // Creating ANF Account
            //----------------------            
            WriteConsoleMessage($"Requesting ANF Account to be created in {location}");
            var newAccount = await Creation.CreateOrUpdateANFAccountAsync(anfClient,
                resourceGroupName,
                location,
                anfAccountName,
                domainJoinUsername,
                domainJoinPassword,
                dnsList,
                adFQDN,
                smbServerNamePrefix,
                encodedCertContent);
            WriteConsoleMessage($"\tAccount Resource Id: {newAccount.Id}");

            //----------------------
            // Creating ANF Capacity Pool
            //----------------------            
            WriteConsoleMessage($"Requesting ANF Primary Capacity Pool to be created in {location}");
            var newPrimaryPool = await Creation.CreateOrUpdateANFCapacityPoolAsync(anfClient, resourceGroupName, location, anfAccountName, capacityPoolName, capacitypoolSize, capacityPoolServiceLevel);
            WriteConsoleMessage($"\tCapacity Pool Resource Id: {newPrimaryPool.Id}");

            //----------------------
            // Creating ANF Volume 
            //----------------------            
            WriteConsoleMessage($"Requesting ANF Volume to be created in {location}");
            var newVolume = await Creation.CreateOrUpdateANFVolumeAsync(anfClient, resourceGroupName, location, anfAccountName, capacityPoolName, capacityPoolServiceLevel, anfVolumeName, subnetId, volumeSize);
            WriteConsoleMessage($"\tVolume Resource Id: {newVolume.Id}");

            WriteConsoleMessage($"Waiting for {newVolume.Id} to be available...");
            await ResourceUriUtils.WaitForAnfResource<Volume>(anfClient, newVolume.Id);

            //--------------------
            // Clean Up Resources
            //--------------------

            if (shouldCleanUp)
            {
                WriteConsoleMessage("-------------------------");
                WriteConsoleMessage("Cleaning up ANF resources");
                WriteConsoleMessage("-------------------------");

                // Deleting Volume in the secondary pool first
                WriteConsoleMessage("Deleting Dual-Protocol Volume...");
                await Deletion.DeleteANFVolumeAsync(anfClient, resourceGroupName, anfAccountName, capacityPoolName, anfVolumeName);
                await ResourceUriUtils.WaitForNoAnfResource<Volume>(anfClient, newVolume.Id);

                // Deleting Primary Pool
                WriteConsoleMessage("Deleting Capacity Pool...");
                await Deletion.DeleteANFCapacityPoolAsync(anfClient, resourceGroupName, anfAccountName, capacityPoolName);
                await ResourceUriUtils.WaitForNoAnfResource<CapacityPool>(anfClient, newPrimaryPool.Id);

                //Deleting Account
                WriteConsoleMessage("Deleting Account ...");
                await Deletion.DeleteANFAccountAsync(anfClient, resourceGroupName, anfAccountName);
                await ResourceUriUtils.WaitForNoAnfResource<NetAppAccount>(anfClient, newAccount.Id);
            }
        }
    }
}

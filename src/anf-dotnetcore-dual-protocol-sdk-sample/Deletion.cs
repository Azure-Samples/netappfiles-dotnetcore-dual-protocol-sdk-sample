// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

namespace Microsoft.Azure.Management.ANF.Samples
{
    using Microsoft.Azure.Management.NetApp;
    using Microsoft.Azure.Management.NetApp.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Includes deletion functionality for ANF resources
    /// </summary>
    public class Deletion
    {
        /// <summary>
        /// Deletes Azure NetApp Files Account
        /// </summary>
        /// <param name="anfClient">Client object</param>
        /// <param name="resourceGroupName">Resource group name</param>
        /// <param name="accountName">Azure NetApp Files account name</param>
        public static async Task DeleteANFAccountAsync(AzureNetAppFilesManagementClient anfClient, string resourceGroupName, string accountName)
        {
            await anfClient.Accounts.DeleteAsync(resourceGroupName, accountName);
        }

        /// <summary>
        /// Deletes Azure NetApp Files Capacity Pool
        /// </summary>
        /// <param name="anfClient">Client object</param>
        /// <param name="resourceGroupName">Resource group name</param>
        /// <param name="accountName">Azure NetApp Files Account name</param>
        /// <param name="poolName">Azure NetApp Files Capacity Pool name</param>
        public static async Task DeleteANFCapacityPoolAsync(AzureNetAppFilesManagementClient anfClient, string resourceGroupName, string accountName, string poolName)
        {
            await anfClient.Pools.DeleteAsync(resourceGroupName, accountName, poolName);
        }

        /// <summary>
        /// Deletes Azure NetApp Files Volume
        /// </summary>
        /// <param name="anfClient">Client object</param>
        /// <param name="resourceGroupName">Resource group name</param>
        /// <param name="accountName">Azure NetApp Files Account name</param>
        /// <param name="poolName">Azure NetApp Files Capacity Pool name</param>
        /// <param name="volumeName">Azure NetApp Files Volume</param>
        public static async Task DeleteANFVolumeAsync(AzureNetAppFilesManagementClient anfClient, string resourceGroupName, string accountName, string poolName, string volumeName)
        {
            await anfClient.Volumes.DeleteAsync(resourceGroupName, accountName, poolName, volumeName);
        }
    }
}

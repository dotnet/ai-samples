// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace FormApplication;

public partial class FillApplicationFunction
{
    private string? name = null;
    private string? email = null;
    private string? phone = null;
    private string? address = null;
    private bool? receiveUpdates = null;

    [Function]
    /// <summary>
    /// Save progress once a piece of information is provided.
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="email">email</param>
    /// <param name="phone">phone</param>
    /// <param name="address">address</param>
    /// <param name="receiveUpdates">if user wants to receive updates</param>
    public async Task<string> SaveProgress(
        string name = "",
        string email = "",
        string phone = "",
        string address = "",
        bool? receiveUpdates = null)
    {
        this.name = !string.IsNullOrEmpty(name) ? name : this.name;
        this.email = !string.IsNullOrEmpty(email) ? email : this.email;
        this.phone = !string.IsNullOrEmpty(phone) ? phone : this.phone;
        this.address = !string.IsNullOrEmpty(address) ? address : this.address;
        this.receiveUpdates = receiveUpdates ?? this.receiveUpdates;

        var missingInformationStringBuilder = new StringBuilder();
        if (string.IsNullOrEmpty(this.name))
        {
            missingInformationStringBuilder.AppendLine("Name is missing.");
        }

        if (string.IsNullOrEmpty(this.email))
        {
            missingInformationStringBuilder.AppendLine("Email is missing.");
        }

        if (string.IsNullOrEmpty(this.phone))
        {
            missingInformationStringBuilder.AppendLine("Phone is missing.");
        }

        if (string.IsNullOrEmpty(this.address))
        {
            missingInformationStringBuilder.AppendLine("Address is missing.");
        }

        if (this.receiveUpdates == null)
        {
            missingInformationStringBuilder.AppendLine("ReceiveUpdates is missing.");
        }

        if (missingInformationStringBuilder.Length > 0)
        {
            return await Task.FromResult(missingInformationStringBuilder.ToString());
        }
        else
        {
            return "Application information is saved to database.";
        }
    }
}

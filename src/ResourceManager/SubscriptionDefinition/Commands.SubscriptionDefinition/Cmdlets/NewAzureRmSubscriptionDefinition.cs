﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
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

using Microsoft.Azure.Commands.SubscriptionDefinition.Common;
using Microsoft.Azure.Commands.SubscriptionDefinition.Models;
using Microsoft.Azure.Management.ResourceManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Microsoft.Azure.Commands.SubscriptionDefinition.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "AzureRmSubscriptionDefinition", SupportsShouldProcess = true), OutputType(typeof(PSSubscriptionDefinition))]
    public class NewAzureRmSubscriptionDefinition : AzureSubscriptionDefinitionCmdletBase
    {
        [Parameter(Mandatory = true, HelpMessage = "Id of the management group in which to create the subscription definition.")]
        public Guid ManagementGroupId { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Name of the subscription definition.")]
        public string Name { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Offer type of the subscription definition.")]
        [ArgumentCompleter(typeof(OfferTypeCompleter))]
        public RuntimeDefinedParameter OfferType { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Display name of the subscription.")]
        public string SubscriptionDisplayName { get; set; }

        public override void ExecuteCmdlet()
        {
            if (this.ShouldProcess(target: this.Name, action: "Create subscription definition"))
            {
                this.WriteSubscriptionDefinitionObject(this.SubscriptionDefinitionClient.SubscriptionDefinitions.Create(new Microsoft.Azure.Management.ResourceManager.Models.SubscriptionDefinition(
                    groupId: this.ManagementGroupId.ToString(),
                    name: this.Name,
                    ratingContext: new Management.ResourceManager.Models.SubscriptionDefinitionPropertiesRatingContext(this.OfferType.Value.ToString()),
                    subscriptionDisplayName: this.SubscriptionDisplayName ?? this.Name)));
            }
        }

        private class OfferTypeCompleter : IArgumentCompleter
        {
            private static readonly string[] KnownOfferTypes = { "MS-AZR-0017P", "MS-AZR-0148P" };

            public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
            {
                var pattern = new WildcardPattern(wordToComplete + "*", WildcardOptions.IgnoreCase);
                return KnownOfferTypes
                    .Where(pattern.IsMatch)
                    .Select(s => new CompletionResult(s));
            }
        }
    }
}

// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RolesToPermission;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests
{
    public class TestAuthorizationPolicyPerformance
    {
        private ITestOutputHelper _output;

        public TestAuthorizationPolicyPerformance(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ComparePerformance()
        {
            //SETUP
            var random = new Random(1);
            const int numTimes = 1000;

            var original = new OriginalAuthorizationPolicyProvider(
                new OptionsWrapper<AuthorizationOptions>(new AuthorizationOptions()));
            var simplified = new SimplifiedAuthorizationPolicyProvider(
                new OptionsWrapper<AuthorizationOptions>(new AuthorizationOptions()));

            for (int i = 0; i < 10; i++)
            {
                await original.GetPolicyAsync(random.Next(1, 10).ToString("D10"));
                await simplified.GetPolicyAsync(random.Next(1, 10).ToString("D10"));
            }

            //ATTEMPT
            using (new TimeThings(_output, "Original", numTimes))
            {
                for (int i = 0; i < numTimes; i++)
                {
                    await original.GetPolicyAsync(random.Next(1, 40).ToString("D10"));
                } 
            }
            using (new TimeThings(_output, "Simplified", numTimes))
            {
                for (int i = 0; i < numTimes; i++)
                {
                    await simplified.GetPolicyAsync(random.Next(1, 40).ToString("D10"));
                }
            }

            //VERIFY

        }
    }


    public class OriginalAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public OriginalAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();

                // Add policy to the AuthorizationOptions, so we don't have to re-create it each time
                _options.AddPolicy(policyName, policy);
            }
            return policy;
        }
    }

    public class SimplifiedAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public SimplifiedAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;

        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName) ?? new AuthorizationPolicyBuilder()
                             .AddRequirements(new PermissionRequirement(policyName))
                             .Build();

            return policy ;
        }
    }
}
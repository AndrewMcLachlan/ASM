using System;
using System.Net;
using Asm.Tests.Common;
using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Net.Tests
{
    [Binding]
    [Scope(Feature = "IPAddress Extensions")]
    public class IPAddressExtensionsSteps
    {
        private IPAddress _ipAddress;
        private IPAddress _subnetMask;

        [Given(@"I have an IP Address '(.*)'")]
        public void GivenIHaveAnIPAddress(string ipAddress)
        {
            _ipAddress = IPAddress.Parse(ipAddress);
        }

        [Given(@"I have a subnet mask '(.*)'")]
        public void GivenIHaveASubnetMask(string subnetMask)
        {
            _subnetMask = IPAddress.Parse(subnetMask);
        }

        [When(@"I call ToCidrString")]
        public void WhenICallToCidrString()
        {
            ScenarioContext.Current.Add("Result", _ipAddress.ToCidrString(_subnetMask));
        }

        [When(@"I call ToCidrString expecting an exception")]
        public void WhenICallToCidrStringWithException()
        {
            SpecFlowHelper.CatchException(() => _ipAddress.ToCidrString(_subnetMask));
        }

        [When(@"I call ToUInt32")]
        public void WhenICallToUInt32()
        {
            ScenarioContext.Current.Add("Result", _ipAddress.ToUInt32());
        }

        [When(@"I call ToUInt32 expecting an exception")]
        public void WhenICallToUInt32WithException()
        {
            SpecFlowHelper.CatchException(() => _ipAddress.ToUInt32());
        }

        [Then(@"the string value '(.*)' will be returned")]
        public void ThenTheStringValue_WillBeReturned(string expected)
        {
            var result = ScenarioContext.Current.Get<string>("Result");
            Assert.Equal(expected, result);
        }

        [Then(@"the unsigned 32 bit integer value (.*) will be returned")]
        public void ThenTheUnsigned32BitIntegerValue_WillBeReturned(uint expected)
        {
            var result = ScenarioContext.Current.Get<uint>("Result");

            Assert.Equal(expected, result);
        }
    }
}

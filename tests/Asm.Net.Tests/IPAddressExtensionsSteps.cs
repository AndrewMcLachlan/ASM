using System;
using System.Net;
using Asm.Testing;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Configuration;
using Xunit;

namespace Asm.Net.Tests
{
    [Binding]
    [Scope(Feature = "IPAddress Extensions")]
    public class IPAddressExtensionsSteps
    {
        private ScenarioData _scenarioData;
        private ScenarioResult<Exception> _exception;

        public class ScenarioData
        {
            public IPAddress IPAddress { get; set; }
            public IPAddress SubnetMask { get; set; }

            public string Cidr { get; set; }

            public UInt32 IPAddressAsUInt32 { get; set; }
        }

        public IPAddressExtensionsSteps(ScenarioData scenarioData, ScenarioResult<Exception> exception)
        {
            _scenarioData = scenarioData;
            _exception = exception;
        }

        [Given(@"I have an IP Address '(.*)'")]
        public void GivenIHaveAnIPAddress(string ipAddress)
        {
            _scenarioData.IPAddress = IPAddress.Parse(ipAddress);
        }

        [Given(@"I have a subnet mask '(.*)'")]
        public void GivenIHaveASubnetMask(string subnetMask)
        {
            _scenarioData.SubnetMask = IPAddress.Parse(subnetMask);
        }

        [When(@"I call ToCidrString")]
        public void WhenICallToCidrString()
        {
            //ScenarioContext.Current.Add("Result", );
            _scenarioData.Cidr = _scenarioData.IPAddress.ToCidrString(_scenarioData.SubnetMask);
        }

        [When(@"I call ToCidrString expecting an exception")]
        public void WhenICallToCidrStringWithException()
        {
            SpecFlowHelper.CatchException(() => _scenarioData.IPAddress.ToCidrString(_scenarioData.SubnetMask), _exception);
        }

        [When(@"I call ToUInt32")]
        public void WhenICallToUInt32()
        {
            _scenarioData.IPAddressAsUInt32 = _scenarioData.IPAddress.ToUInt32();
        }

        [When(@"I call ToUInt32 expecting an exception")]
        public void WhenICallToUInt32WithException()
        {
            SpecFlowHelper.CatchException(() => _scenarioData.IPAddress.ToUInt32(), _exception);
        }

        [Then(@"the string value '(.*)' will be returned")]
        public void ThenTheStringValue_WillBeReturned(string expected)
        {
            Assert.Equal(expected, _scenarioData.Cidr);
        }

        [Then(@"the unsigned 32 bit integer value (.*) will be returned")]
        public void ThenTheUnsigned32BitIntegerValue_WillBeReturned(uint expected)
        {
            Assert.Equal(expected, _scenarioData.IPAddressAsUInt32);
        }
    }
}

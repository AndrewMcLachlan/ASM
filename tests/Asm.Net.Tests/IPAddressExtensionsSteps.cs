using System;
using System.Net;
using Asm.Testing;
using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Net.Tests;

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

        public uint IPAddressAsUInt32 { get; set; }
    }

    public IPAddressExtensionsSteps(ScenarioData scenarioData, ScenarioResult<Exception> exception)
    {
        _scenarioData = scenarioData;
        _exception = exception;
    }

    [Given(@"I have an IP Address '(.*)'")]
    public void GivenIHaveAnIPAddress(IPAddress ipAddress)
    {
        _scenarioData.IPAddress = ipAddress;
    }

    [Given(@"I have a subnet mask '(.*)'")]
    public void GivenIHaveASubnetMask(IPAddress subnetMask)
    {
        _scenarioData.SubnetMask = subnetMask;
    }

    [Given(@"I have an unsigned 32 bit integer (.*)")]
    public void GivenIHaveAnUnsignedBitInteger(uint ipAddressAsUint32)
    {
        _scenarioData.IPAddressAsUInt32 = ipAddressAsUint32;
    }

    [When(@"I call ToCidrString")]
    public void WhenICallToCidrString()
    {
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

    [When(@"I call FromUInt32")]
    public void WhenICallFromUInt()
    {
        _scenarioData.IPAddress = IPAddressExtensions.FromUInt32(_scenarioData.IPAddressAsUInt32);
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

    [Then(@"the IP Address (.*) is returned")]
    public void ThenTheIPAddress_IsReturned(IPAddress ipAddress)
    {
        Assert.Equal(ipAddress, _scenarioData.IPAddress);
    }

}

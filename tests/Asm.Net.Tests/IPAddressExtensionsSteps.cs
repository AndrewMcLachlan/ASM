using System;
using System.Net;
using Asm.Testing;
using TechTalk.SpecFlow;
using Xunit;

namespace Asm.Net.Tests;

[Binding]
[Scope(Feature = "IPAddress Extensions")]
public class IPAddressExtensionsSteps(ScenarioContext context, IPAddressExtensionsSteps.ScenarioData scenarioData)
{
    public class ScenarioData
    {
        public IPAddress IPAddress { get; set; }
        public IPAddress SubnetMask { get; set; }

        public string Cidr { get; set; }

        public uint IPAddressAsUInt32 { get; set; }
    }

    [Given(@"I have an IP Address '(.*)'")]
    public void GivenIHaveAnIPAddress(IPAddress ipAddress)
    {
        scenarioData.IPAddress = ipAddress;
    }

    [Given(@"I have a subnet mask '(.*)'")]
    public void GivenIHaveASubnetMask(IPAddress subnetMask)
    {
        scenarioData.SubnetMask = subnetMask;
    }

    [Given(@"I have an unsigned 32 bit integer (.*)")]
    public void GivenIHaveAnUnsignedBitInteger(uint ipAddressAsUint32)
    {
        scenarioData.IPAddressAsUInt32 = ipAddressAsUint32;
    }

    [When(@"I call ToCidrString")]
    public void WhenICallToCidrString()
    {
        scenarioData.Cidr = scenarioData.IPAddress.ToCidrString(scenarioData.SubnetMask);
    }

    [When(@"I call ToCidrString expecting an exception")]
    public void WhenICallToCidrStringWithException()
    {
        context.CatchException(() => scenarioData.IPAddress.ToCidrString(scenarioData.SubnetMask));
    }

    [When(@"I call ToUInt32")]
    public void WhenICallToUInt32()
    {
        scenarioData.IPAddressAsUInt32 = scenarioData.IPAddress.ToUInt32();
    }

    [When(@"I call ToUInt32 expecting an exception")]
    public void WhenICallToUInt32WithException()
    {
        context.CatchException(() => scenarioData.IPAddress.ToUInt32());
    }

    [When(@"I call FromUInt32")]
    public void WhenICallFromUInt()
    {
        scenarioData.IPAddress = IPAddressExtensions.FromUInt32(scenarioData.IPAddressAsUInt32);
    }

    [Then(@"the string value '(.*)' will be returned")]
    public void ThenTheStringValue_WillBeReturned(string expected)
    {
        Assert.Equal(expected, scenarioData.Cidr);
    }

    [Then(@"the unsigned 32 bit integer value (.*) will be returned")]
    public void ThenTheUnsigned32BitIntegerValue_WillBeReturned(uint expected)
    {
        Assert.Equal(expected, scenarioData.IPAddressAsUInt32);
    }

    [Then(@"the IP Address (.*) is returned")]
    public void ThenTheIPAddress_IsReturned(IPAddress ipAddress)
    {
        Assert.Equal(ipAddress, scenarioData.IPAddress);
    }

}

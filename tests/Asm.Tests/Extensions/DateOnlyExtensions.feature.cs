﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Asm.Tests.Extensions
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class DateOnlyExtensionsFeature : object, Xunit.IClassFixture<DateOnlyExtensionsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "DateOnlyExtensions.feature"
#line hidden
        
        public DateOnlyExtensionsFeature(DateOnlyExtensionsFeature.FixtureData fixtureData, Asm_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Extensions", "DateOnlyExtensions", "  In order to use DateOnly extension methods\r\n  As a developer\r\n  I want to have " +
                    "unit tests for the DateOnly extension methods", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Get today\'s date")]
        [Xunit.TraitAttribute("FeatureTitle", "DateOnlyExtensions")]
        [Xunit.TraitAttribute("Description", "Get today\'s date")]
        [Xunit.TraitAttribute("Category", "Unit")]
        public void GetTodaysDate()
        {
            string[] tagsOfScenario = new string[] {
                    "Unit"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get today\'s date", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 7
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 8
    testRunner.When("I get today\'s date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 9
    testRunner.Then("the result should be today\'s date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Convert DateOnly to DateTime at start of day")]
        [Xunit.TraitAttribute("FeatureTitle", "DateOnlyExtensions")]
        [Xunit.TraitAttribute("Description", "Convert DateOnly to DateTime at start of day")]
        [Xunit.TraitAttribute("Category", "Unit")]
        public void ConvertDateOnlyToDateTimeAtStartOfDay()
        {
            string[] tagsOfScenario = new string[] {
                    "Unit"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Convert DateOnly to DateTime at start of day", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 12
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 13
    testRunner.Given("I have a date \"2023-10-01\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 14
    testRunner.When("I convert the date to DateTime at the start of the day", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 15
    testRunner.Then("the DateTime result should be \"2023-10-01T00:00:00\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Convert DateOnly to DateTime at end of day")]
        [Xunit.TraitAttribute("FeatureTitle", "DateOnlyExtensions")]
        [Xunit.TraitAttribute("Description", "Convert DateOnly to DateTime at end of day")]
        [Xunit.TraitAttribute("Category", "Unit")]
        public void ConvertDateOnlyToDateTimeAtEndOfDay()
        {
            string[] tagsOfScenario = new string[] {
                    "Unit"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Convert DateOnly to DateTime at end of day", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 18
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 19
    testRunner.Given("I have a date \"2023-10-01\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 20
    testRunner.When("I convert the date to DateTime at the end of the day", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 21
    testRunner.Then("the DateTime result should be \"2023-10-01T23:59:59.9999999\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Calculate difference in months between two dates")]
        [Xunit.TraitAttribute("FeatureTitle", "DateOnlyExtensions")]
        [Xunit.TraitAttribute("Description", "Calculate difference in months between two dates")]
        [Xunit.TraitAttribute("Category", "Unit")]
        [Xunit.InlineDataAttribute("2023-01-01", "2023-01-01", "0", new string[0])]
        [Xunit.InlineDataAttribute("2023-01-01", "2023-02-01", "1", new string[0])]
        [Xunit.InlineDataAttribute("2023-02-01", "2023-01-01", "1", new string[0])]
        [Xunit.InlineDataAttribute("2023-01-01", "2022-12-01", "1", new string[0])]
        [Xunit.InlineDataAttribute("2023-01-01", "2022-01-01", "12", new string[0])]
        [Xunit.InlineDataAttribute("2023-01-01", "2023-01-02", "0", new string[0])]
        [Xunit.InlineDataAttribute("2023-10-01", "2022-08-15", "13", new string[0])]
        [Xunit.InlineDataAttribute("2022-08-15", "2023-10-01", "13", new string[0])]
        [Xunit.InlineDataAttribute("2023-08-01", "2022-10-15", "9", new string[0])]
        [Xunit.InlineDataAttribute("2022-10-15", "2023-08-01", "9", new string[0])]
        public void CalculateDifferenceInMonthsBetweenTwoDates(string date, string otherDate, string result, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "Unit"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Date", date);
            argumentsOfScenario.Add("Other Date", otherDate);
            argumentsOfScenario.Add("Result", result);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Calculate difference in months between two dates", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 24
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 25
    testRunner.Given(string.Format("I have a date \"{0}\"", date), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 26
    testRunner.And(string.Format("I have another date \"{0}\"", otherDate), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
    testRunner.When("I calculate the difference in months between the dates", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 28
    testRunner.Then(string.Format("the integer result should be {0}", result), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                DateOnlyExtensionsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                DateOnlyExtensionsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
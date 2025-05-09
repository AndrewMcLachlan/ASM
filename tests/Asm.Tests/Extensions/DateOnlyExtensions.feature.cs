﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Asm.Tests.Extensions
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class DateOnlyExtensionsFeature : object, Xunit.IClassFixture<DateOnlyExtensionsFeature.FixtureData>, Xunit.IAsyncLifetime
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Extensions", "DateOnlyExtensions", "  In order to use DateOnly extension methods\r\n  As a developer\r\n  I want to have " +
                "unit tests for the DateOnly extension methods", global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "DateOnlyExtensions.feature"
#line hidden
        
        public DateOnlyExtensionsFeature(DateOnlyExtensionsFeature.FixtureData fixtureData, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
        }
        
        public static async System.Threading.Tasks.Task FeatureSetupAsync()
        {
        }
        
        public static async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
        }
        
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(featureHint: featureInfo);
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Equals(featureInfo) == false)))
            {
                await testRunner.OnFeatureEndAsync();
            }
            if ((testRunner.FeatureContext == null))
            {
                await testRunner.OnFeatureStartAsync(featureInfo);
            }
        }
        
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
            global::Reqnroll.TestRunnerManager.ReleaseTestRunner(testRunner);
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
        {
            await this.TestInitializeAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
        {
            await this.TestTearDownAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Get today\'s date")]
        [Xunit.TraitAttribute("FeatureTitle", "DateOnlyExtensions")]
        [Xunit.TraitAttribute("Description", "Get today\'s date")]
        [Xunit.TraitAttribute("Category", "Unit")]
        public async System.Threading.Tasks.Task GetTodaysDate()
        {
            string[] tagsOfScenario = new string[] {
                    "Unit"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Get today\'s date", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 7
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 8
    await testRunner.WhenAsync("I get today\'s date", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 9
    await testRunner.ThenAsync("the result should be today\'s date", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Convert DateOnly to DateTime at start of day")]
        [Xunit.TraitAttribute("FeatureTitle", "DateOnlyExtensions")]
        [Xunit.TraitAttribute("Description", "Convert DateOnly to DateTime at start of day")]
        [Xunit.TraitAttribute("Category", "Unit")]
        public async System.Threading.Tasks.Task ConvertDateOnlyToDateTimeAtStartOfDay()
        {
            string[] tagsOfScenario = new string[] {
                    "Unit"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Convert DateOnly to DateTime at start of day", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 12
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 13
    await testRunner.GivenAsync("I have a date \"2023-10-01\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 14
    await testRunner.WhenAsync("I convert the date to DateTime at the start of the day", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 15
    await testRunner.ThenAsync("the DateTime result should be \"2023-10-01T00:00:00\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Convert DateOnly to DateTime at end of day")]
        [Xunit.TraitAttribute("FeatureTitle", "DateOnlyExtensions")]
        [Xunit.TraitAttribute("Description", "Convert DateOnly to DateTime at end of day")]
        [Xunit.TraitAttribute("Category", "Unit")]
        public async System.Threading.Tasks.Task ConvertDateOnlyToDateTimeAtEndOfDay()
        {
            string[] tagsOfScenario = new string[] {
                    "Unit"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Convert DateOnly to DateTime at end of day", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 18
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 19
    await testRunner.GivenAsync("I have a date \"2023-10-01\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 20
    await testRunner.WhenAsync("I convert the date to DateTime at the end of the day", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 21
    await testRunner.ThenAsync("the DateTime result should be \"2023-10-01T23:59:59.9999999\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
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
        public async System.Threading.Tasks.Task CalculateDifferenceInMonthsBetweenTwoDates(string date, string otherDate, string result, string[] exampleTags)
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
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Calculate difference in months between two dates", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 24
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 25
    await testRunner.GivenAsync(string.Format("I have a date \"{0}\"", date), ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 26
    await testRunner.AndAsync(string.Format("I have another date \"{0}\"", otherDate), ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 27
    await testRunner.WhenAsync("I calculate the difference in months between the dates", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 28
    await testRunner.ThenAsync(string.Format("the integer result should be {0}", result), ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : object, Xunit.IAsyncLifetime
        {
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
            {
                await DateOnlyExtensionsFeature.FeatureSetupAsync();
            }
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
            {
                await DateOnlyExtensionsFeature.FeatureTearDownAsync();
            }
        }
    }
}
#pragma warning restore
#endregion

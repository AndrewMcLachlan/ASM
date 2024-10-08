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
namespace Asm.Tests
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class StringExtensionsFeature : object, Xunit.IClassFixture<StringExtensionsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "StringExtensions.feature"
#line hidden
        
        public StringExtensionsFeature(StringExtensionsFeature.FixtureData fixtureData, Asm_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "", "String Extensions", null, ProgrammingLanguage.CSharp, featureTags);
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
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Append")]
        [Xunit.TraitAttribute("FeatureTitle", "String Extensions")]
        [Xunit.TraitAttribute("Description", "Append")]
        [Xunit.TraitAttribute("Category", "Unit")]
        [Xunit.InlineDataAttribute("", "Hello", "Hello", new string[0])]
        [Xunit.InlineDataAttribute("Hello", "World", "Hello,World", new string[0])]
        public void Append(string @string, string append, string value, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "Unit"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("string", @string);
            argumentsOfScenario.Add("append", append);
            argumentsOfScenario.Add("value", value);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Append", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 4
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
    testRunner.Given(string.Format("I have a string \'{0}\'", @string), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 6
    testRunner.And("I have a separator \',\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 7
    testRunner.When(string.Format("I Append \'{0}\' to the string", append), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 8
    testRunner.Then(string.Format("the string value \'{0}\' is returned", value), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Prepend")]
        [Xunit.TraitAttribute("FeatureTitle", "String Extensions")]
        [Xunit.TraitAttribute("Description", "Prepend")]
        [Xunit.TraitAttribute("Category", "Unit")]
        [Xunit.InlineDataAttribute("", "Hello", "Hello", new string[0])]
        [Xunit.InlineDataAttribute("Hello", "World", "World,Hello", new string[0])]
        public void Prepend(string @string, string prepend, string value, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "Unit"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("string", @string);
            argumentsOfScenario.Add("prepend", prepend);
            argumentsOfScenario.Add("value", value);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Prepend", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 16
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 17
    testRunner.Given(string.Format("I have a string \'{0}\'", @string), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 18
    testRunner.And("I have a separator \',\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 19
    testRunner.When(string.Format("I Prepend \'{0}\' to the string", prepend), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 20
    testRunner.Then(string.Format("the string value \'{0}\' is returned", value), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Squish")]
        [Xunit.TraitAttribute("FeatureTitle", "String Extensions")]
        [Xunit.TraitAttribute("Description", "Squish")]
        [Xunit.TraitAttribute("Category", "Unit")]
        public void Squish()
        {
            string[] tagsOfScenario = new string[] {
                    "Unit"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Squish", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 28
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 29
    testRunner.Given("I have a string \'Hello World\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 30
    testRunner.When("I Squish the string by 2 characters", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 31
    testRunner.Then("the string value \'llo Wor\' is returned", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Convert string to title case")]
        [Xunit.TraitAttribute("FeatureTitle", "String Extensions")]
        [Xunit.TraitAttribute("Description", "Convert string to title case")]
        [Xunit.TraitAttribute("Category", "Unit")]
        public void ConvertStringToTitleCase()
        {
            string[] tagsOfScenario = new string[] {
                    "Unit"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Convert string to title case", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 34
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 35
    testRunner.Given("I have a string \'hello world\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 36
    testRunner.When("I convert the string to title case", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 37
    testRunner.Then("the string value \'Hello World\' is returned", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="ToMachine")]
        [Xunit.TraitAttribute("FeatureTitle", "String Extensions")]
        [Xunit.TraitAttribute("Description", "ToMachine")]
        [Xunit.TraitAttribute("Category", "Unit")]
        [Xunit.InlineDataAttribute("Hello, World", "hello-world", new string[0])]
        [Xunit.InlineDataAttribute("Hello & World", "hello-world", new string[0])]
        [Xunit.InlineDataAttribute("Hello- !\"£$%^&*()[]{}<>,./?;:\'@#~\\World", "hello-world", new string[0])]
        public void ToMachine(string @string, string value, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "Unit"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("string", @string);
            argumentsOfScenario.Add("value", value);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("ToMachine", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 40
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 41
    testRunner.Given(string.Format("I have a string \'{0}\'", @string), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 42
    testRunner.When("I convert the string to machine format", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 43
    testRunner.Then(string.Format("the string value \'{0}\' is returned", value), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
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
                StringExtensionsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                StringExtensionsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion

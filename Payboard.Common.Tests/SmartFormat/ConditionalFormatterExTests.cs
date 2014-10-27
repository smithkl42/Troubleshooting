using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payboard.Common.SmartFormat;
using Should;
using SmartFormat;
using SmartFormat.Extensions;

namespace Payboard.Common.Tests.SmartFormat
{
    [TestClass]
    public class ConditionalFormatterExTests
    {
        private SmartFormatter _formatter;
        private TestPerson _person;

        [TestInitialize]
        public void Setup()
        {
            _formatter = new SmartFormatter();
            _formatter.AddExtensions(
                new ReflectionSource(_formatter),
                new DictionarySource(_formatter),
                new DefaultSource(_formatter));
            _formatter.AddExtensions(
                new PluralLocalizationFormatter("en"),
                new ConditionalFormatterEx(), 
                new DefaultFormatterEx(true));
            _person = new TestPerson();
        }

        [TestMethod]
        public void ConditionalFormatting_ShouldHandleSimpleConditions()
        {
            // This actually gets handled by the PluralLocalizationFormatter, but same difference.
            const string s = "{Age} {Age:year|years}";
            _person.Age = 1;
            _formatter.Format(s, _person).ShouldEqual("1 year");
            _person.Age = 2;
            _formatter.Format(s, _person).ShouldEqual("2 years");
        }

        [TestMethod]
        public void ConditionalFormatting_ShouldHandleComplexNumericConditions()
        {
            const string s = "{Age::>=55?Senior Citizen|>=30?Adult|>=18?Young Adult|>12?Teenager|>2?Child|Baby}";
            _person.Age = 55;
            _formatter.Format(s, _person).ShouldEqual("Senior Citizen");
            _person.Age = 3;
            _formatter.Format(s, _person).ShouldEqual("Child");
            _person.Age = 2;
            _formatter.Format(s, _person).ShouldEqual("Baby");
        }

        [TestMethod]
        public void ConditionalFormatting_ShouldHandleNullStrings()
        {
            const string s = "{Name::{}<br>|}";
            _person.Name = "Bob";
            _formatter.Format(s, _person).ShouldEqual("Bob<br>");
            _person.Name = null;
            _formatter.Format(s, _person).ShouldEqual("");
        }

        [TestMethod]
        public void ConditionalFormatting_ShouldHandleNullObjects()
        {
            const string s = "{TestObject::NotNull|Null}";
            _person.TestObject = new object();
            _formatter.Format(s, _person).ShouldEqual("NotNull");
            _person.TestObject = null;
            _formatter.Format(s, _person).ShouldEqual("Null");
        }
    }

    public class TestPerson
    {
        public decimal Age { get; set; }
        public string Name { get; set; }
        public object TestObject { get; set; }
    }
}

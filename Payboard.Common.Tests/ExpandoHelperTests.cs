using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Payboard.Model;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class ExpandoHelperTests
    {
        [TestMethod]
        public void ToExpando_ShouldSetSimpleProperties_ToLowerCase()
        {
            var source = new ExpandoSource();
            dynamic target = source.ToExpando();
            var dict = (IDictionary<string, object>)target;
            dict["name"].ShouldEqual(source.Name);
            dict["expandosourceid"].ShouldEqual(source.ExpandoSourceId);
            dict["double"].ShouldEqual(source.Double);
            dict["guid"].ShouldEqual(source.Guid);
        }

        [TestMethod]
        public void ToExpando_WithRetainCase_ShouldRetainCase()
        {
            var source = new ExpandoSource();
            dynamic target = source.ToExpando(true);
            var dict = (IDictionary<string, object>)target;
            dict["Name"].ShouldEqual(source.Name);
            dict["ExpandoSourceId"].ShouldEqual(source.ExpandoSourceId);
            dict["Double"].ShouldEqual(source.Double);
            dict["Guid"].ShouldEqual(source.Guid);
        }

        [TestMethod]
        public void ToExpando_ShouldNotSetComplexProperties()
        {
            var source = new ExpandoSource();
            dynamic target = source.ToExpando();
            var dict = (IDictionary<string, object>)target;
            dict.ContainsKey("object").ShouldBeFalse();
            dict.ContainsKey("child").ShouldBeFalse();
            dict.ContainsKey("nullobject").ShouldBeFalse();
        }

        [TestMethod]
        public void ToFullExpando_ShouldSetComplexProperties()
        {
            var source = new ExpandoSource();
            dynamic target = source.ToFullExpando();
            var dict = (IDictionary<string, object>)target;
            dict["name"].ShouldEqual(source.Name);
            dict["object"].ShouldEqual(source.Object);
            dict["expandosourceid"].ShouldEqual(source.ExpandoSourceId);
            dict["double"].ShouldEqual(source.Double);
            dict["child"].ShouldEqual(source.Child);
            dict["guid"].ShouldEqual(source.Guid);
            dict["nullableint"].ShouldEqual(source.NullableInt);
            dict["nullobject"].ShouldBeNull();
        }

        //    [TestMethod]
        //    public void ToExpando_ShouldWorkWithMessageData()
        //    {
        //        var md = new MessageData();
        //        md.Campaign = new ExpandoObject();
        //        md.CampaignStep = new ExpandoObject();
        //        md.Customer = new ExpandoObject();
        //        md.CustomerUser = new ExpandoObject();
        //        md.Organization = new ExpandoObject();
        //        dynamic dyn = md.ToFullExpando(true);
        //        Assert.AreEqual(md.Campaign, dyn.Campaign);
        //        Assert.AreEqual(md.CampaignStep, dyn.CampaignStep);
        //        Assert.AreEqual(md.Customer, dyn.Customer);
        //        Assert.AreEqual(md.CustomerUser, dyn.CustomerUser);
        //        Assert.AreEqual(md.Organization, dyn.Organization);
        //    }
    }

    public class ExpandoSource
    {
        public ExpandoSource()
        {
            Name = Guid.NewGuid().ToString();
            Object = new object();
            ExpandoSourceId = int.MaxValue;
            Double = double.MinValue;
            NullableInt = 100;
            Child = this;
            Guid = Guid.NewGuid();
        }

        public string Name { get; set; }
        public object Object { get; set; }
        public object NullObject { get; set; }
        public int ExpandoSourceId { get; set; }
        public double Double { get; set; }
        public int? NullableInt { get; set; }
        public ExpandoSource Child { get; set; }
        public Guid Guid { get; set; }
    }
}
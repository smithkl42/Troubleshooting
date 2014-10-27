using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class TypeHelperTests
    {
        private SomeTypedClass _source;
        private SomeTypedClass _target;

        [TestInitialize]
        public void Setup()
        {
            _source = new SomeTypedClass();
            _source.IntegerProperty = 1;
            _source.NullableIntegerProperty = 1;
            _source.StringProperty = "Some string";
            _source.ReferenceProperty = _source;
            _target = new SomeTypedClass();
        }

        [TestMethod]
        public void ShallowClone_ShouldCopyIntegers()
        {
            var copy = _source.ShallowClone();
            copy.IntegerProperty.ShouldEqual(_source.IntegerProperty);
        }

        [TestMethod]
        public void ShallowClone_ShouldCopyNullableIntegers()
        {
            var copy = _source.ShallowClone();
            copy.NullableIntegerProperty.ShouldEqual(_source.NullableIntegerProperty);
        }

        [TestMethod]
        public void ShallowClone_ShouldCopyStrings()
        {
            var copy = _source.ShallowClone();
            copy.StringProperty.ShouldEqual(_source.StringProperty);
        }

        [TestMethod]
        public void ShallowClone_ShouldNotCopyReferences()
        {
            var copy = _source.ShallowClone();
            copy.ReferenceProperty.ShouldBeNull();
        }

        [TestMethod]
        public void ShallowClone_ShouldReturnNullIfSourceIsNull()
        {
            SomeTypedClass source = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            var copy = source.ShallowClone();
            copy.ShouldBeNull();
        }

        [TestMethod]
        public void ShallowCopyTo_ShouldCopyIntegers()
        {
            _source.ShallowCopyTo(_target);
            _target.IntegerProperty.ShouldEqual(_source.IntegerProperty);
        }

        [TestMethod]
        public void ShallowCopyTo_ShouldCopyNullableIntegers()
        {
            _source.ShallowCopyTo(_target);
            _target.NullableIntegerProperty.ShouldEqual(_source.NullableIntegerProperty);
        }

        [TestMethod]
        public void ShallowCopyTo_ShouldCopyStrings()
        {
            _source.ShallowCopyTo(_target);
            _target.StringProperty.ShouldEqual(_source.StringProperty);
        }

        [TestMethod]
        public void ShallowCopyTo_ShouldNotCopyReferences()
        {
             _source.ShallowCopyTo(_target);
            _target.ReferenceProperty.ShouldBeNull();
        }

        [TestMethod]
        public void ShallowCopyTo_ShouldNotCopyExceptions()
        {
            _source.ShallowCopyTo(_target, "IntegerProperty");
            _target.ReferenceProperty.ShouldBeNull();
            _target.IntegerProperty.ShouldEqual(0);
            _target.NullableIntegerProperty.ShouldEqual(_source.NullableIntegerProperty);
        }

        [TestMethod]
        public void ShallowCopyTo_ShouldNotCopyExceptionExpressions()
        {
            _source.ShallowCopyTo(_target, x => x.IntegerProperty);
            _target.ReferenceProperty.ShouldBeNull();
            _target.IntegerProperty.ShouldEqual(0);
            _target.NullableIntegerProperty.ShouldEqual(_source.NullableIntegerProperty);
        }

        [TestMethod]
        public void GetFriendlyTypeName_ShouldHandleSimpleTypes()
        {
            typeof(string).GetFriendlyTypeName().ShouldEqual("String");
        }

        [TestMethod]
        public void GetFriendlyTypeName_ShouldHandleComplexTypes()
        {
            typeof(Dictionary<string, object>).GetFriendlyTypeName()
                .ShouldEqual("Dictionary<String, Object>");
        }

        [TestMethod]
        public void GetFriendlyTypeName_ShouldHandleReallyComplexTypes()
        {
            typeof(Dictionary<string, Dictionary<string, object>>).GetFriendlyTypeName()
                .ShouldEqual("Dictionary<String, Dictionary<String, Object>>");
        }

    }

    public class SomeTypedClass
    {
        public int IntegerProperty { get; set; }
        public int? NullableIntegerProperty { get; set; }
        public string StringProperty { get; set; }
        public SomeTypedClass ReferenceProperty { get; set; }

        public string ReadOnlyStringProperty
        {
            get { return StringProperty; }
        }

        public int ReadOnlyIntegerProperty
        {
            get { return IntegerProperty; }
        }
    }
}
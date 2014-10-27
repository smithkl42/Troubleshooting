using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class NumberHelperTests
    {
        [TestMethod]
        public void Retries_ShouldOnlyExecuteOnce_IfNoErrors()
        {
            var executions = 0;
            5.Retries(x =>
            {
                executions++;
            });
            executions.ShouldEqual(1);
        }

        [TestMethod]
        public void Retries_ShouldExecuteMaxNumberOfTimes_IfErrors()
        {
            var executions = 0;
            try
            {
                5.Retries(x =>
                {
                    executions++;
                    throw new Exception();
                });
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
            executions.ShouldEqual(5);
        }

        [TestMethod]
        public void Retries_ShouldThrowFinalException_IfErrors()
        {
            bool exceptionThrown = false;
            try
            {
                5.Retries(x =>
                {
                    throw new Exception(x.ToString());
                });
            }
            catch(Exception ex)
            {
                exceptionThrown = true;
                ex.Message.ShouldEqual("4");
            }
            exceptionThrown.ShouldEqual(true);
        }

    }
}

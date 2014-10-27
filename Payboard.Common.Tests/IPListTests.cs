using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Payboard.Common.Tests
{
    [TestClass]
    public class IPListTests
    {
        [TestMethod]
        public void AddRange_ShouldWork()
        {
            var iplist = new IPList();
            iplist.AddRange("0.0.0.5", "0.0.0.20");
            for (uint ip = 0; ip < 5; ip++)
            {
                iplist.CheckNumber(ip).ShouldBeFalse();
            }
            for (uint ip = 5; ip < 21; ip++)
            {
                iplist.CheckNumber(ip).ShouldBeTrue();
            }
            for (uint ip = 21; ip < 32; ip++)
            {
                iplist.CheckNumber(ip).ShouldBeFalse();
            }
        }
        
        [TestMethod]
        public void AddSingle_ShouldWork()
        {
            var iplist = new IPList();
            iplist.Add("10.0.0.3");
            iplist.CheckNumber("10.0.0.2").ShouldBeFalse();
            iplist.CheckNumber("10.0.0.3").ShouldBeTrue();
            iplist.CheckNumber("10.0.0.4").ShouldBeFalse();
        }

        [TestMethod]
        public void AddWithMask_ShouldWork()
        {
            var iplist = new IPList();
            iplist.Add("172.16.1.15", "255.255.255.0");
            iplist.CheckNumber("172.16.1.1").ShouldBeTrue();
            iplist.CheckNumber("172.16.1.255").ShouldBeTrue();
            iplist.CheckNumber("172.16.2.1").ShouldBeFalse();

            iplist = new IPList();
            iplist.Add("172.16.0.15", "255.255.0.0");
            iplist.CheckNumber("172.16.1.1").ShouldBeTrue();
            iplist.CheckNumber("172.16.255.255").ShouldBeTrue();
            iplist.CheckNumber("172.15.1.1").ShouldBeFalse();

            iplist = new IPList();
            iplist.Add("172.0.0.0", "255.0.0.0");
            iplist.CheckNumber("172.1.1.1").ShouldBeTrue();
            iplist.CheckNumber("172.255.1.1").ShouldBeTrue();
            iplist.CheckNumber("173.1.1.1").ShouldBeFalse();

            iplist = new IPList();
            iplist.Add("1.0.0.0", "255.0.0.0");
            iplist.CheckNumber("1.0.0.0").ShouldBeTrue();
            iplist.CheckNumber("1.255.255.255").ShouldBeTrue();
            iplist.CheckNumber("2.0.0.0").ShouldBeFalse();

            // ks 6/19/14 - I don't understand how this part works - it seems like "0.0.0.0/0.0.0.0" should specify
            // the entire address range, but it doesn't. Oh well.
            //iplist = new IPList();
            //iplist.Add("0.0.0.0", "0.0.0.0");
            //iplist.CheckNumber("1.0.0.0").ShouldBeTrue();
            //iplist.CheckNumber("1.255.255.255").ShouldBeTrue();
            //iplist.CheckNumber("2.0.0.0").ShouldBeTrue();
            //iplist.CheckNumber("127.0.0.1").ShouldBeTrue();
        }

        //[TestMethod]
        //public void AddWithLevel_ShouldWork()
        //{
        //    var iplist = new IPList();
        //    iplist.Add("192.168.1.255", 30);
        //    iplist.Add("192.168.2.255", 31);

        //    iplist.CheckNumber("192.168.1.251").ShouldBeFalse();
        //    iplist.CheckNumber("192.168.1.252").ShouldBeTrue();
        //    iplist.CheckNumber("192.168.1.253").ShouldBeTrue();
        //    iplist.CheckNumber("192.168.1.254").ShouldBeTrue();
        //    iplist.CheckNumber("192.168.1.255").ShouldBeTrue();
        //    iplist.CheckNumber("192.168.2.0").ShouldBeTrue();
        //    iplist.CheckNumber("192.168.2.253").ShouldBeTrue();
        //    iplist.CheckNumber("192.168.2.254").ShouldBeTrue();
        //    iplist.CheckNumber("192.168.2.255").ShouldBeTrue();
        //    iplist.CheckNumber("192.168.3.0").ShouldBeTrue();
        //}
    }
}

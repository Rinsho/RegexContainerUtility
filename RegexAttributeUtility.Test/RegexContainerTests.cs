using System;
using System.Collections.Generic;
using NUnit.Framework;
using RegularExpression.Utility;
using System.Text.RegularExpressions;

namespace RegexAttributeUtility.Test
{
    [TestFixture]
    public class RegexContainerTests
    {       
        [RegexContainer(@"(?<Part1>\S+)\s(?<Part2>\d+)")]
        class ValidContainer
        {
            [RegexData]
            public string Part1;

            [RegexData]
            public int Part2 { get; set; }

            [RegexData(MatchID = "Random")]
            public float Part3 { get; private set; }

            [RegexData]
            private int Part4;

            [RegexData]
            [RegexDataList(',')]
            public List<int> List1;

            [RegexData]
            [RegexDataList('|')]
            public string[] List2 { get; private set; }
        }

        [RegexContainer(@"(?<Addr>[^,]+), ?(?<City>[A-Za-z ]+), ?(?<State>[A-Za-z]{2}) (?<Zip>\d+)")]
        class AddressInfo
        {
            [RegexData(MatchID = "Addr")]
            public string StreetAddress { get; private set; }
            [RegexData]
            public string City { get; private set; }
            [RegexData]
            public string State { get; private set; }
            [RegexData(MatchID = "Zip")]
            public int? ZipCode { get; private set; }
        }

        [RegexContainer(@"(?<FirstName>[A-Za-z]+) (?<LastName>[A-Za-z]+), ?(?<Addr>[^\r\n]+)\r?$", Options = RegexOptions.Multiline)]
        class UserInfo
        {
            [RegexData]
            public string FirstName { get; private set; }
            [RegexData]
            public string LastName { get; private set; }
            [RegexData(MatchID = "Addr")]
            [RegexDataList('|')]
            public List<AddressInfo> Addresses { get; private set; }
        }

        [RegexContainer(@"(?<Names>.+)")]
        class NamesTest
        {
            [RegexData]
            [RegexDataList(',')]
            public string[] Names;
        }

        [RegexContainer(@"(\d+) (\d+) (\d+)")]
        class DefaultGroupingTest
        {
            [RegexData(MatchID = "1")]
            public int X;
            [RegexData(MatchID = "2")]
            public int Y;
            [RegexData(MatchID = "3")]
            public int Z;
        }

        [RegexContainer(@".+")]
        class NonWriteableProperty
        {
            [RegexData]
            public int X { get; }
        }

        [RegexContainer(@"(?<container>.+)")]
        class DataMismatchSubcontainer
        {
            [RegexContainer(@"(?<X>\d+)")]
            public class SubContainer
            {
                [RegexData]
                public int X;
            }

            [RegexData]
            public SubContainer container;
        }

        [RegexContainer(@"(?<Test>.+)")]
        class DataMismatch
        {
            [RegexData]
            public int Test;
        }

        enum TestEnum { Test1 = 0, Test2 = 1 }

        [RegexContainer(@"(?<Value>.+)")]
        class EnumTest
        {
            [RegexData]
            public TestEnum Value;
        }

        [Test]
        public void RegexContainer_ValidContainer_Created()
        {
            RegexContainer<ValidContainer> container = new RegexContainer<ValidContainer>();
            Assert.That(container, Is.Not.Null);
        }

        [Test]
        public void RegexContainer_Parse_ValidContainerReturned()
        {
            RegexContainer<UserInfo> container = new RegexContainer<UserInfo>();
            string text = "John Doe, 1500 St. Martin Blvd, Blah, XX 11111|23 X Pkwy, Test Blank, YY 44444\r\n";
            ContainerResult<UserInfo> result = container.Parse(text);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Value.FirstName, Is.EqualTo("John"));
                Assert.That(result.Value.LastName, Is.EqualTo("Doe"));
                Assert.That(result.Value.Addresses[0].City, Is.EqualTo("Blah"));
                Assert.That(result.Value.Addresses[0].State, Is.EqualTo("XX"));
                Assert.That(result.Value.Addresses[1].StreetAddress, Is.EqualTo("23 X Pkwy"));
                Assert.That(result.Value.Addresses[1].ZipCode, Is.EqualTo(44444));
            });         
        }

        [Test]
        public void RegexContainer_ParseAllCollectionType_ValidContainersReturned()
        {
            string userInfoData =
                "John Doe, 1500 St. Martin Blvd, Blah, XX 11111|23 X Pkwy, Test Blank, YY 44444\nJane Doe, 43 Test Dr. Apt#1, Test, YY 33333";
            RegexContainer<UserInfo> container = new RegexContainer<UserInfo>();
            ContainerResultCollection<UserInfo> results = container.ParseAll(userInfoData);
            Assert.Multiple(() =>
            {
                foreach (UserInfo result in results)
                {
                    Assert.That(result.LastName, Is.EqualTo("Doe"));
                }
            });
        }

        [Test]
        public void RegexContainer_ParseAllArrayType_ValidContainersReturned()
        {
            string names = "John Doe, Jane Doe";
            RegexContainer<NamesTest> container = new RegexContainer<NamesTest>();
            ContainerResult<NamesTest> result = container.Parse(names);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Value.Names[0], Is.EqualTo("John Doe"));
                Assert.That(result.Value.Names[1], Is.EqualTo("Jane Doe"));
            });
        }

        [Test]
        public void RegexContainer_ParseDefaultGrouping_ValidContainerReturned()
        {
            string points = "1 10 7";
            RegexContainer<DefaultGroupingTest> container = new RegexContainer<DefaultGroupingTest>();
            ContainerResult<DefaultGroupingTest> result = container.Parse(points);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Value.X, Is.EqualTo(1));
                Assert.That(result.Value.Y, Is.EqualTo(10));
                Assert.That(result.Value.Z, Is.EqualTo(7));
            });
        }

        [Test]
        public void RegexContainer_DataMismatchWithSubContainer_InvalidContainerReturned()
        {
            RegexContainer<DataMismatchSubcontainer> container = new RegexContainer<DataMismatchSubcontainer>();
            Assert.That(container.Parse("Fail").Success, Is.False);
        }

        [Test]
        public void RegexContainer_DataMismatch_InvalidContainerReturned()
        {
            string text = "Fail";
            RegexContainer<DataMismatch> container = new RegexContainer<DataMismatch>();
            ContainerResult<DataMismatch> result = container.Parse(text);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void RegexContainer_NonWriteableProperty_DataException()
        {
            try
            {
                RegexContainer<NonWriteableProperty> container = new RegexContainer<NonWriteableProperty>();
            }
            catch (TypeInitializationException ex) when (ex.InnerException is InvalidRegexDataException)
            {
                Assert.Pass();
                return;
            }
            Assert.Fail();
        }        

        [Test]
        public void RegexContainer_ValidEnumName_ContainerCreated()
        {
            string name = "Test1";
            RegexContainer<EnumTest> container = new RegexContainer<EnumTest>();
            ContainerResult<EnumTest> result = container.Parse(name);
            Assert.That(result.Value.Value, Is.EqualTo(TestEnum.Test1));
        }

        [Test]
        public void RegexContainer_ValidEnumValue_ContainerCreated()
        {
            string val = "1";
            RegexContainer<EnumTest> container = new RegexContainer<EnumTest>();
            ContainerResult<EnumTest> result = container.Parse(val);
            Assert.That(result.Value.Value, Is.EqualTo(TestEnum.Test2));
        }

        [Test]
        public void RegexContainer_InvalidEnumValue_InvalidContainerReturned()
        {
            string val = "2";
            RegexContainer<EnumTest> container = new RegexContainer<EnumTest>();
            ContainerResult<EnumTest> result = container.Parse(val);
            Assert.That(result.Success, Is.False);
        }
    }
}

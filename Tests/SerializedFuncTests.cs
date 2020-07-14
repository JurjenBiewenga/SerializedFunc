using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;

namespace SerializedFuncImpl.Tests
{
    public class SerializedFuncTests
    {
        [TestCase("Test Value")]
        public void ReturnStringValueNoParamsTest(string value)
        {
            var serializedFunc = new SerializedFunc<string>();
            serializedFunc.Set(() => value);

            Assert.AreEqual(serializedFunc.Invoke(), value);
        }

        [TestCase(1)]
        public void ReturnSimpleValueTypeNoParamsTest(int value)
        {
            var serializedFunc = new SerializedFunc<string, int>();
            serializedFunc.Set(x => value);

            Assert.AreEqual(serializedFunc.Invoke("test"), value);
        }

        [TestCase("test", 10)]
        public void ReturnValueTypeNoParamsTest(string testString, int testInt)
        {
            TestStruct value = new TestStruct(testString, testInt);
            var serializedFunc = new SerializedFunc<TestStruct>();
            serializedFunc.Set(() => value);

            Assert.AreEqual(serializedFunc.Invoke(), value);
        }

        [TestCase("test")]
        public void SetMethodInfoTest(string value)
        {
            var serializedFunc = new SerializedFunc<string, string>();
            var method = typeof(TestSo).GetMethod("Test");
            var testSo = ScriptableObject.CreateInstance<TestSo>();
            serializedFunc.Set(testSo, method);

            Assert.AreEqual(serializedFunc.Invoke(value), value);
        }

        [TestCase()]
        public void IncorrectParametersCountTest()
        {
            var serializedFunc = new SerializedFunc<string, string, string>();
            var method = typeof(TestSo).GetMethod("Test");
            var testSo = ScriptableObject.CreateInstance<TestSo>();
            Assert.Throws<ArgumentException>(() => serializedFunc.Set(testSo, method), "Invalid parameter count");
        }
        
        [TestCase]
        public void IncorrectParametersTest()
        {
            var serializedFunc = new SerializedFunc<bool, string>();
            var method = typeof(TestSo).GetMethod("Test");
            var testSo = ScriptableObject.CreateInstance<TestSo>();
            Assert.Throws<ArgumentException>(() => serializedFunc.Set(testSo, method), "Incorrect first parameter");
        }

        class TestSo : ScriptableObject
        {
            public string Test(string val1)
            {
                return val1;
            }
        }

        private struct TestStruct
        {
            public TestStruct(string testString, int testInt)
            {
                TestString = testString;
                TestInt = testInt;
            }

            public string TestString { get; }
            public int TestInt { get; }
        }
    }
}
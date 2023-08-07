



namespace Chat
{


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  TESTING SECTION - This is all stub code for now. Needs to be updated for actual unit testing of this application. 
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    using Microsoft.VisualStudio.TestTools.UnitTesting;
    /// /////////////
    using System;
    /// /////////////
    class classForUnitTesting
    {

        //  data
        public int age { get; set; }
        public string name { get; set; }

        //  methods

        public classForUnitTesting(int Age, string Name)
        {
            age = Age;

            name = Name;
        }

        ~classForUnitTesting() { }

        public int changeAge()
        {

            int newAge = age + 26;

            age = newAge;

            return newAge;
        }

        public bool printLatestAge()
        {

            Console.WriteLine(" latest age: " + age);

            return true;
        }

    }


    [TestClass]
    public class testClass
    {

        classForUnitTesting CFU;

        [TestInitialize]
        public void Initialize()
        {
            //classForUnitTesting CFU = new classForUnitTesting(25, "david ");
            CFU = new classForUnitTesting(25, "david ");
            CFU.changeAge();



        }

        [TestMethod]
        public void checkAssertions()
        {

            Assert.AreEqual(50, CFU.age);

            return;
        }



    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  END OF TESTING SECTION 
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////




}
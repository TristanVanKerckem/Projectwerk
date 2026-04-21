using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ProjectBeheerUnitTest
{
    [TestClass]
    public class BouwfirmaTest
    {
        public class UnitTest_BouwfirmaValidContructor
        {
            [Theory]
            [InlineData("Project", "project@hot.be", "0478123456")]
            [InlineData("Bouwgroep", "contact@bouw.com", "091234567")]
            public void Test_Constructor_Valid(string naam, string email, string telefoon)
            {
                Bouwfirma firma = new Bouwfirma(naam, email, telefoon);
                Assert.Equal(naam, firma.Naam);
                Assert.Equal(email, firma.Email);
                Assert.Equal(telefoon, firma.TelefoonNummer);
            }
        }
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Test_Naam_Fout(string naam)
        {
            Assert.Throws<BouwfirmaException>(() => new Bouwfirma(naam, "project@hot.be", "04123456789"));
        }
        [Theory]
        [InlineData("projecthot.be")] 
        [InlineData("project@hotebe")]
        [InlineData("")]
        [InlineData(null)]
        public void Test_Email_Invalid(string email)
        {
            Assert.Throws<BouwfirmaException>(() => new Bouwfirma("Acme NV", email, "0478123456"));
        }

        [Theory]
        [InlineData("047ABC456")]     
        [InlineData("")]
        public void Test_Telefoon_Invalid(string telefoon)
        {
            Assert.Throws<BouwfirmaException>(() => new Bouwfirma("project", "project@hot.be", telefoon));
        }
    }
}

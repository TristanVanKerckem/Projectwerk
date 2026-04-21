using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ProjectBeheerUnitTest
{
    [TestClass]
    public class GebruikerTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(999)]
        public void Test_Id_Valid(int id)
        {
            Gebruiker gebruiker = new Gebruiker("Jan Janssen", "jan@janssen.be", false);
            gebruiker.Id = id;
            Assert.Equal(id, gebruiker.Id);
        }
        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_Id_Invalid (int id) {
            Gebruiker gebruiker = new Gebruiker("Jan Janssen", "jan@janssen.be", false);
            Assert.Throws<GebruikerException>(() => gebruiker.Id = id);
        }
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Test_Naam_Invalid(string naam)
        {
            Assert.Throws<GebruikerException>(() => new Gebruiker(naam, "jan@janssen.be", false));
        }
        [Theory]
        [InlineData("janjanssen.be")]   
        [InlineData("jan@janssenbe")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Test_Email_Invalid(string email)
        {
            Assert.Throws<GebruikerException>(() => new Gebruiker("Jan Janssen", email, false));
        }

        [Theory]
        [InlineData("jan@janssen.be")]
        [InlineData("contact@stad.gent.be")]
        public void Test_Email_Valid(string email)
        {
            Gebruiker gebruiker = new Gebruiker("Jan Janssen", email, false);
            Assert.Equal(email, gebruiker.Email);
        }
        [Fact]
        public void Test_IsBeheerder_True()
        {
            Gebruiker gebruiker = new Gebruiker("Jan Janssen", "jan@janssen.be", true);
            Assert.True(gebruiker.IsBeheerder);
        }

        [Fact]
        public void Test_IsBeheerder_False()
        {
            Gebruiker gebruiker = new Gebruiker("Jan Janssen", "jan@janssen.be", false);
            Assert.False(gebruiker.IsBeheerder);
        }
    }

}


using AppointmentSystem.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.UnitTests
{
    public class UnitTestBase
    {
        private readonly IServiceCollection ServiceCollection = new ServiceCollection();
        protected Context _context;
        protected Mock<T> RegisterMock<T>() where T : class
        {
            var mock = new Mock<T>();

            ServiceCollection.AddSingleton(typeof(T), mock.Object);

            return mock;
        }

        protected void Register<I, T>() where I : class where T : class, I
         => ServiceCollection.AddSingleton<I, T>();

        protected I GetService<I>() where I : class
          => ServiceCollection.BuildServiceProvider().GetService<I>();

        protected void RegisterObject<Tp, T>(Tp type, T objeto) where Tp : Type where T : class
           => ServiceCollection.AddSingleton(type, objeto);

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            ConfigureInMemoryDataBase();
        }


        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            _context.Dispose();
        }

        private void ConfigureInMemoryDataBase()
        {
            var options = new DbContextOptionsBuilder<Context>()
                              .UseInMemoryDatabase("InMemoryDatabase")
            .Options;

            _context = new Context(options);

            if (_context.Database.IsInMemory())
                _context.Database.EnsureDeleted();
        }
    }
}

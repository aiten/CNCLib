using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxxon.Repository.Context;
using Proxxon.Repository;
using Proxxon.Repository.Entities;
using System.Threading.Tasks;

namespace Proxxon.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        private const string _connectionString = @"Data Source=c:\tmp\Database.sdf";
        private ProxxonRepository _repository;

        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {           
            //drop and recreate the test Db everytime the tests are run. 
            AppDomain.CurrentDomain.SetData("DataDirectory", testContext.TestDeploymentDir);

            using(var context = new ProxxonContext(_connectionString))
            {
				System.Data.Entity.Database.SetInitializer<ProxxonContext>(new ProxxonInitializer());
                context.Database.Initialize(true);        
            }        
        }

        [TestInitialize]
        public void Init()
        {
			this._repository = new ProxxonRepository(new ProxxonContext(_connectionString));
        }

        [TestMethod]
        public void QueryAuthorsToListReturnsAllAuthors()
        {
            var machines = _repository.Query<Machine>().ToList();

			Assert.AreEqual(4, machines.Count);
        }
/*
        [TestMethod]
        public async Task QueryAuthorsToListAsyncReturnsAllAuthors()
        {
            var authors = await repository.Query<Author>().ToListAsync();

            Assert.AreEqual(4, authors.Count);
        }

        [TestMethod]
        public void FilteredQueryAuthorsToListReturnsSomeAuthors()
        {
            var authors = repository.Query<Author>().Where(a => a.Name != "Orwell").ToList();

            Assert.AreEqual(3, authors.Count);
        }

        [TestMethod]
        public async Task FilteredQueryAuthorsToListAsyncReturnsSomeAuthors()
        {
            var authors = await repository.Query<Author>().Where(a => a.Name != "Orwell").ToListAsync();

            Assert.AreEqual(3, authors.Count);
        }

        [TestMethod]
        public void UnfilteredFirstOrDefaultResultNotNull()
        {
            var author = repository.Query<Author>().FirstOrDefault();

            Assert.IsNotNull(author);
        }

        [TestMethod]
        public async Task UnfilteredFirstOrDefaultAsyncResultNotNull()
        {
            var author = await repository.Query<Author>().FirstOrDefaultAsync();

            Assert.IsNotNull(author);
        }

        [TestMethod]
        public void FilteredFirstOrDefaultResultIsCorrect()
        {
            var author = repository.Query<Author>().Where(a => a.Name == "Orwell").FirstOrDefault();

            Assert.AreEqual("Orwell", author.Name);
        }

        [TestMethod]
        public async Task FilteredFirstOrDefaultAsyncResultIsCorrect()
        {
            var author = await repository.Query<Author>().Where(a => a.Name == "Orwell").FirstOrDefaultAsync();

            Assert.AreEqual("Orwell", author.Name);
        }

        [TestMethod]
        public void WithoutIncludePublisherNavPropIsNotLoaded()
        {
            var book = repository.Query<Book>().Where(b => b.Title == "The Wasp Factory").FirstOrDefault();

            Assert.IsNull(book.Publisher);
        }

        [TestMethod]
        public void WithIncludePublisherNavPropIsLoaded()
        {
            var book = repository.Query<Book>().Where(b => b.Title == "The Wasp Factory").Include(b => b.Publisher).FirstOrDefault();

            Assert.IsNotNull(book.Publisher);
        }

        [TestMethod]
        public void WithoutIncludeAuthorsNavColIsNotLoaded()
        {
            var book = repository.Query<Book>().Where(b => b.Title == "The Wasp Factory").FirstOrDefault();

            Assert.AreEqual(0,book.Authors.Count);
        }

        [TestMethod]
        public void WithIncludeAuthorsNavColIsLoaded()
        {
            var book = repository.Query<Book>().Where(b => b.Title == "The Wasp Factory").Include(b => b.Authors).FirstOrDefault();

            Assert.AreEqual(1,book.Authors.Count);
        }

        [TestMethod]
        public void AuthorsOrderByNameIsOrderedAZ()
        {
            var authors = repository.Query<Author>().OrderBy(b => b.Name).ToList();

            Assert.IsTrue(authors[0].Name == "Banks" & authors[1].Name == "Gibson" & authors[2].Name == "Orwell" & authors[3].Name == "Stirling");
        }

        [TestMethod]
        public void AuthorsOrderByDescendingNameIsOrderedZA()
        {
            var authors = repository.Query<Author>().OrderByDescending(b => b.Name).ToList();

            Assert.IsTrue(authors[3].Name == "Banks" & authors[2].Name == "Gibson" & authors[1].Name == "Orwell" & authors[0].Name == "Stirling");
        }

        [TestMethod]
        public void AuthorsPagingWorksPage1()
        {
            var authors = repository.Query<Author>().OrderBy(b => b.Name).Page(0,2).ToList();

            Assert.IsTrue(authors.Count == 2 & authors[0].Name == "Banks" & authors[1].Name == "Gibson");
        }

        [TestMethod]
        public void AuthorsPagingWorksPage2()
        {
            var authors = repository.Query<Author>().OrderBy(b => b.Name).Page(1, 2).ToList();

            Assert.IsTrue(authors.Count == 2 & authors[0].Name == "Orwell" & authors[1].Name == "Stirling");
        }
 */
	}
}

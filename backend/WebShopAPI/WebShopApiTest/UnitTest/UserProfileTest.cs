using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopApiTest.UnitTest
{
    public class UserProfileTest
    {
        private WebShopContext _context;
        private IUserProfileService _userProfileService;

        [SetUp]
        public void SetUp()
        {
            var option = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;

            _context = new WebShopContext(option);
            
        }
    }
}

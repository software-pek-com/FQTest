using AutoMapper;
using MoWebApp;

namespace Tests
{
    public class TestBase
    {
        public TestBase()
        {
            Mapper = Startup.ConfigureMapper();
        }

        public IMapper Mapper { get; private set; }
    }
}

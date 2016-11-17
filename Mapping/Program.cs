namespace Mapping
{
    using System;
    using System.Linq;
    using AutoMapper;
    using StructureMap;

    internal class Program
    {
        private static void Main()
        {
            IoC.Container.Configure(cfg => { cfg.AddRegistry(new AutoMapperRegistry()); });

            using (var container = IoC.Container.GetNestedContainer())
            {
                var registerViewModel = new RegisterViewModel
                {
                    Email = "Test@mail.com",
                    Login = "Login",
                    Password = "pwd",
                    ConfirmPassword = "pwd",
                    FirstName = "Kos",
                    LastName = "Kos",
                    BirthDay = DateTime.UtcNow
                };

                var config = container.GetInstance<MapperConfiguration>();

                var mapper = config.CreateMapper();

                var model = mapper.Map<ApplicationUser>(registerViewModel);
            }
        }
    }

    public class AutoMapperRegistry : Registry
    {
        public AutoMapperRegistry()
        {
            var profiles =
                typeof(AutoMapperRegistry).Assembly.GetTypes()
                    .Where(t => typeof(Profile).IsAssignableFrom(t))
                    .Select(t => (Profile) Activator.CreateInstance(t));

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            For<MapperConfiguration>().Use(config);
            For<IMapper>().Use(ctx => ctx.GetInstance<MapperConfiguration>().CreateMapper(ctx.GetInstance));
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, UserInfo>();

            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(dest => dest.UserInfo,
                    opt => opt.MapFrom(src => src));
        }
    }

    public static class IoC
    {
        static IoC()
        {
            Container = new Container();
        }

        public static IContainer Container { get; set; }
    }

    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDay { get; set; }
    }

    public class ApplicationUser
    {
        public virtual UserInfo UserInfo { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime BirthDay { get; set; }
        public string MiddleName { get; set; }
    }
}
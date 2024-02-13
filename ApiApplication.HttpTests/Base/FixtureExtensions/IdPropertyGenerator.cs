using AutoFixture.Kernel;

namespace ApiApplication.HttpTests.Base.FixtureExtensions
{
    public class IdPropertyGenerator : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as System.Reflection.PropertyInfo;
            if (pi != null && pi.Name == "Id" && pi.PropertyType == typeof(int))
            {
                // Customize how Id property is generated
                return 0; // For example, always return 1 as the Id value
            }
            return new NoSpecimen();
        }
    }
}
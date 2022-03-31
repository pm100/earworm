using System.Reflection;
namespace EarWorm.Pages
{
    public partial class About
    {
        public string Version() {
            var assm = Assembly.GetExecutingAssembly();
        
            var fv = assm.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var avi = assm.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return avi.InformationalVersion.ToString();
        }
    }
}

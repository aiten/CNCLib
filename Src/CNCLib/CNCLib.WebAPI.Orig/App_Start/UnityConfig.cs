using System;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.WebAPI.Controllers;
using Framework.Web;
using Unity;

namespace CNCLib.WebAPI
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = ((Framework.Tools.Dependency.UnityDependencyContainer) Framework.Tools.Dependency.Dependency.Container).MyUnityContainer;
              /*
                            var container = new UnityContainer();
              */
              RegisterTypes(container);
              return container;

          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            container.RegisterType<CambamController>();
            container.RegisterType<EepromConfigurationController>();
            container.RegisterType<GCodeController>();
            container.RegisterType<GCodeLoadHelper>();
            container.RegisterType<HomeController>();
            container.RegisterType<ItemController>();
            container.RegisterType<IRest<Item>, ItemRest>();
            container.RegisterType<LoadOptionsController>();
            container.RegisterType<IRest<LoadOptions>, LoadInfoRest>();
            container.RegisterType<MachineController>();
            container.RegisterType<IRest<Machine>, MachineRest>();
        }
    }
}
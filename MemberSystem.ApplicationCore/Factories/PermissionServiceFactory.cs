using MemberSystem.ApplicationCore.Interfaces.Services;
using MemberSystem.ApplicationCore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Factories
{
    public class PermissionServiceFactory : IPermissionServiceFactory
    {
        //private readonly IServiceProvider _serviceProvider;

        //public PermissionServiceFactory(IServiceProvider serviceProvider)
        //{
        //    _serviceProvider = serviceProvider;
        //}

        //public IPermissionService GetService(string type)
        //{
        //    return type switch
        //    {
        //        "Role" => _serviceProvider.GetService<RolePermissionService>(),
        //        "Position" => _serviceProvider.GetService<PositionPermissionService>(),
        //        _ => throw new ArgumentException("Invalid service type")
        //    };
        //}
    }

}

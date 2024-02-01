using System.Threading.Tasks;
using Amg.Authentication.Command.Authorization.Groups;
using Amg.Authentication.DomainModel.Modules.Groups;
using Amg.Authentication.DomainModel.Modules.Groups.Interfaces;
using Amg.Authentication.DomainModel.Modules.Permissions.Interfaces;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.CommandHandler.Modules.Authorization
{
    public class GroupCommandHandler : ICommandHandler,
        ICommandHandler<AddGroupCommand>,
        ICommandHandler<AssignPermissionToGroupCommand>,
        ICommandHandler<AssignUserToGroupsCommand>,
        ICommandHandler<UpdateGroupCommand>
    {

        private readonly IGroupRepository _groupRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GroupCommandHandler(IGroupRepository groupRepository, IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork)
        {
            _groupRepository = groupRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(AddGroupCommand command)
        {
            var group = new Group(command.Name);
            await _groupRepository.AddAsync(group);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task HandleAsync(AssignPermissionToGroupCommand command)
        {
            var group = await _groupRepository.GetIncludePermissionsByIdAsync(command.GroupId);
            var permissions = await _permissionRepository.GetByIdsAsync(command.PermissionIds);
            group.RemoveAllPermissions();
            group.AddPermissions(permissions);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task HandleAsync(AssignUserToGroupsCommand command)
        {
            var newGroups = await _groupRepository.GetByIdsAsync(command.GroupIds);
            var oldGroups = await _groupRepository.GetAllUserGroupsAsync(command.UserId);
            oldGroups.ForEach(i => i.Group.RemoveUser(i));
            newGroups.ForEach(i => i.AddUser(command.UserId));
            
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task HandleAsync(UpdateGroupCommand command)
        {
            var group = await _groupRepository.GetByIdAsync(command.Id);
            group.Update(command.Name);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

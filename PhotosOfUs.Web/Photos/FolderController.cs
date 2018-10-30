using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using System.Security.Claims;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Web.Utilities;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Folder")]
    public class FolderApiController : Controller
    {
        private PhotosOfUsContext _context;
        private IRepository<User> _user;

        public FolderApiController(PhotosOfUsContext context, IRepository<User> userRepository)
        {
            _context = context;
            _user = userRepository;
        }

        [HttpPost]
        public FolderViewModel Post(string name)
        {
            var photographer = _user.Find(x => x.Id == User.ID());

            var folder = photographer.AddFolder(name);
            _user.Commit();

            return folder.ToViewModel<FolderViewModel>();
        }
    
        [Route("RenameFolder")]
        [HttpPut]
        public IActionResult Put([FromBody]FolderRenameViewModel model)
        {
            var photographer = _user.Find(x => x.Id == User.ID());

            var folder = photographer.Folder.SingleOrDefault(x=> x.Id == model.Id);
            folder.Name = model.NewName;
            _user.Commit();

            return Ok();
        }

        [HttpPost]
        [Route("DeleteFolder/{id:int}")]
        public IActionResult DeleteFolder(int id)
        {
            var photographer = _user.Find(x => x.Id == User.ID());

            photographer.RemoveFolder(id);
            _user.Commit();

            return Ok();
        }
    }
}
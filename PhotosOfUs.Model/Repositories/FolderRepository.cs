using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Repositories
{
    public class FolderRepository
    {
        private PhotosOfUsContext _context;

        public FolderRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public Folder Add(string name, int photographerId)
        {
            Folder newFolder = new Folder();
            newFolder.Name = name;
            newFolder.CreatedDate = DateTime.Now;
            newFolder.PhotographerId = photographerId;

            _context.Folder.Add(newFolder);
            _context.SaveChanges();

            return newFolder;
        }

        public void Delete(int id)
        {
            Folder folder = _context.Folder.Find(id);
            _context.Folder.Remove(folder);
            _context.SaveChanges();
        }
    }
}

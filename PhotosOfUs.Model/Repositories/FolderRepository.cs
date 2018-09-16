using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //_context.Photo.RemoveRange(folder.Photo);
            //_context.Folder.Remove(folder);
            folder.IsDeleted = true;
            _context.SaveChanges();
        }

        public Folder Rename(int id, string newName, int photographerId)
        {
            var folder = _context.Folder.Include("Photo").SingleOrDefault(x=>x.Id == id);
            folder.Name = newName;

            _context.Entry(folder).State = EntityState.Modified;
            _context.SaveChanges();

            return folder;
        }
    }
}

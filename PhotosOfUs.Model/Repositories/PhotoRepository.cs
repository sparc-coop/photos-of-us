﻿using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.WindowsAzure.Storage.Blob;
using PhotosOfUs.Model.Services;
using System.Threading.Tasks;

namespace PhotosOfUs.Model.Repositories
{
    public class PhotoRepository
    {
        private PhotosOfUsContext _context;

        public PhotoRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public List<Folder> GetFolders(int photographerId)
        {
            return _context.Folder.Include(x => x.Photo).Where(x => x.PhotographerId == photographerId).ToList();
        }

        public Folder GetPhotos(int photographerId, int folderId)
        {
            return _context.Folder.Include(x => x.Photo).Single(x => x.PhotographerId == photographerId && x.Id == folderId);
        }

        public List<Photo> GetPhotosByCode(string code)
        {
            return _context.Photo.Where(x => x.Code.Equals(code)).ToList();
        }

        public Photo GetPhoto(int photoId)
        {
            return _context.Photo.Include(x => x.Photographer).Single(x => x.Id == photoId);
        }

        public List<PrintType> GetPrintTypes()
        {
            return _context.PrintType.ToList();
        }

        public void SavePhoto(Photo photo)
        {
            _context.Photo.Attach(photo);
            _context.SaveChanges();
        }

        public bool IsPhotoCodeAlreadyUsed(int photographerId, string code)
        {
            return _context.Photo.Any(x => x.PhotographerId == photographerId && x.Code == code);
        }

        //public async Task<Photo> Upload(int photographerId, Photo photo, Stream stream, string fileName, string photoName, string photoCode)
        //{
        //    // TODO: Generate the code 
        //    photo.Code = "abcdef";
        //    photo.PhotographerId = photographerId;
        //    photo.UploadDate = DateTime.Now;
        //    photo.Name = fileName;

        //    var extension = Path.GetExtension(fileName);
        //    fileName = Guid.NewGuid() + extension;
        //    photo.Url = await UploadFile(photographerId, stream, photoName, photoCode, extension);

        //    await _context.Photo.AddAsync(photo);

        //    return photo;
        //}

        public async Task<string> UploadFile(int photographerId, Stream stream, string photoName, string photoCode, string extension, int folderId, bool publicProfile = false)
        {
            var urlTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var url = $"{photographerId}/{folderId}/{photoName.Split('.')[0] + urlTimeStamp + extension}";

            stream.Position = 0;
            var container = new MemoryStream();
            var containerBlob = StorageHelpers.Container("photos").GetBlockBlobReference(url);
            containerBlob.Properties.CacheControl = "public, max-age=31556926";
            container.Position = 0;
            await containerBlob.UploadFromStreamAsync(stream);

            // Generate thumbnail
            stream.Position = 0;
            var thumbnail = new MemoryStream();
            ImageHelper.ConvertImageToThumbnailJpg(stream, thumbnail, extension);
            var thumbnailBlob = StorageHelpers.Container("thumbnails").GetBlockBlobReference(url);
            thumbnailBlob.Properties.CacheControl = "public, max-age=31556926";
            thumbnail.Position = 0;
            await thumbnailBlob.UploadFromStreamAsync(thumbnail);

            var photo = new Photo()
            {
                Name = photoName,
                PhotographerId = photographerId,
                UploadDate = DateTime.Now,
                Url = containerBlob.Uri.AbsoluteUri,
                Code = photoCode,
                FolderId = folderId,
                PublicProfile = publicProfile
            };

            _context.Photo.Attach(photo);
            _context.SaveChanges();

            return containerBlob.Uri.AbsoluteUri;
        }

        
        public List<Photo> GetProfilePhotos(int photographerId)
        {
            return _context.Photo.Where(x => x.PublicProfile).ToList();
        }

        public async Task UploadProfilePhotoAsync(int photographerId, FileStream stream, string photoName, string empty, string extension, bool v)
        {
            var public_folder = _context.Folder.Where(x => x.PhotographerId == photographerId && x.Name.ToLower() == "public");

            if(public_folder.Count() == 0)
            {
                Folder pFolder = new FolderRepository(_context).Add("Public", photographerId);
                await UploadFile(photographerId, stream, photoName, string.Empty, extension, pFolder.Id);
            }
            else
            {
                await UploadFile(photographerId, stream, photoName, string.Empty, extension, public_folder.First().Id);
            }
        }
    }

  
}

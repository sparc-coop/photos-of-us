using Microsoft.EntityFrameworkCore;
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
using PhotosOfUs.Model.ViewModels;

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
            return _context.Folder.Include(x => x.Photo).SingleOrDefault(x => x.PhotographerId == photographerId && x.Id == folderId);
        }

        public List<Photo> GetPhotosByCode(string code)
        {
            return _context.Photo.Where(x => x.Code.Equals(code)).ToList();
        }

        public Photo GetPhoto(int photoId)
        {
            return _context.Photo.Include("Photographer").Single(x => x.Id == photoId);
        }

        public void UpdatePrice(int photoId, decimal price)
        {
            Photo photo = _context.Photo.Where(x => x.Id == photoId).FirstOrDefault();
            photo.Price = price;
            _context.Photo.Update(photo);
            _context.SaveChanges();
        }

        public void SavePhoto(Photo photo)
        {
            _context.Photo.Attach(photo);
            _context.SaveChanges();
        }

        public List<Tag> GetAllTags()
        {
            return _context.Tag.ToList();
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

        public async Task<string> UploadFile(int photographerId, Stream stream, string photoName, string photoCode, string extension, int folderId, double? price, bool publicProfile = false)
        {
            var urlTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var url = $"{photographerId}/{folderId}/{photoName.Split('.')[0] + urlTimeStamp + extension}";

            stream.Position = 0;
            var container = new MemoryStream();
            var containerBlob = StorageHelpers.Container("photos").GetBlockBlobReference(url);
            containerBlob.Properties.CacheControl = "public, max-age=31556926";
            container.Position = 0;
            await containerBlob.UploadFromStreamAsync(stream);

            //generate watermark
            stream.Position = 0;
            var watermarkImg = new MemoryStream();
            ImageHelper.AddWatermark(stream, watermarkImg, extension);
            var watermarkBlob = StorageHelpers.Container("watermark").GetBlockBlobReference(url);
            watermarkBlob.Properties.CacheControl = "public, max-age=31556926";
            watermarkImg.Position = 0;
            await watermarkBlob.UploadFromStreamAsync(watermarkImg);
            
         

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
                PublicProfile = publicProfile,
                Price = (decimal)price
            };

            _context.Photo.Attach(photo);
            _context.SaveChanges();

            return containerBlob.Uri.AbsoluteUri;
        }

        
        public List<Photo> GetProfilePhotos(int photographerId)
        {
            return _context.Photo.Where(x => x.PublicProfile && !x.IsDeleted && x.PhotographerId == photographerId).ToList();
        }

        public List<Photo> GetPublicPhotos()
        {
            return _context.Photo.Where(x => x.PublicProfile && !x.IsDeleted).ToList();
        }

        //public List<SearchIndex> GetCases(List<int> citationIds)
        //{
        //    return Context.SearchIndexes.Where(x => citationIds.Contains(x.RecordID)).ToList();
        //}

        public void AddTags(List<TagViewModel> tags)
        {
            foreach (TagViewModel tag in tags)
            {
                Tag newTag = new Tag
                {
                    Name = tag.text
                };
                _context.Tag.Add(newTag);
            }
            _context.SaveChanges();
        }

        public List<Tag> GetTags(string[] tagarray)
        {
            return _context.Tag.Where(x => tagarray.Contains(x.Name)).ToList();
        }

        public List<Photo> GetPublicPhotosByTag(List<Tag> taglist)
        {
            var publicphotos = _context.Photo.Where(x => x.PublicProfile).ToList();

            List<int> tagids = new List<int>();

            foreach (Tag tag in taglist)
            {
                tagids.Add(tag.Id);
            }

            List<PhotoTag> phototags = _context.PhotoTag.Where(x => tagids.Contains(x.TagId)).ToList();

            List<int> ptids = new List<int>();

            foreach (PhotoTag pt in phototags)
            {
                ptids.Add(pt.PhotoId);
            }

            List<Photo> photos = publicphotos.Where(x => ptids.Contains(x.Id)).ToList();

            return photos;
        }

        public List<PhotoTag> GetTagsByPhotos(List<int> photos)
        {
            var tags = new List<Tag>();
            var phototags = new List<PhotoTag>();

            //phototags = _context.PhotoTag.ToList();

            var photoList = new List<Photo>();
            foreach(int id in photos)
            {
                photoList.Where(x => x.Id == id);
            }

            foreach (Photo photo in photoList)
            {
                var tagsfromphoto = _context
                    .PhotoTag
                    .Include(item => item.Tag)
                    .Where(cm => cm.PhotoId == photo.Id)
                    .ToList();

                foreach (PhotoTag tag in tagsfromphoto)
                {
                    phototags.Add(tag);
                }
            }

            //foreach (PhotoTag phototag in phototags)
            //{
            //    tags = _context
            //        .Tag
            //        .Include(item => item.Name)
            //        .Where(cm => cm.Id == phototag.TagId)
            //        .ToList();
            //}

            return phototags;
        }

        public void DeletePhotos(List<Photo> photos)
        {
            foreach (Photo photo in photos)
            {
                var photodb = _context.Photo.Find(photo.Id);
                photodb.IsDeleted = true;
            }
            _context.SaveChanges();
        }

        public async Task UploadProfilePhotoAsync(int photographerId, FileStream stream, string photoName, string empty, double? price, string extension)
        {
            var public_folder = _context.Folder.Where(x => x.PhotographerId == photographerId && x.Name.ToLower() == "public");

            if(public_folder.Count() == 0)
            {
                Folder pFolder = new FolderRepository(_context).Add("Public", photographerId);
                await UploadFile(photographerId, stream, photoName, string.Empty, extension, pFolder.Id, price, true);
            }
            else
            {
                await UploadFile(photographerId, stream, photoName, string.Empty, extension, public_folder.First().Id, price, true);
            }
        }
    }

  
}

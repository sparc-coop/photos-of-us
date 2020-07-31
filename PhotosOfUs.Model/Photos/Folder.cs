﻿using PhotosOfUs.Core.Users;
using System;
using System.Collections.Generic;

namespace PhotosOfUs.Core.Photos
{
    public partial class Folder
    {
        private Folder() { }

        public Folder(User photographer, string name)
        {
            Photographer = photographer ?? throw new ArgumentNullException(nameof(photographer));
            PhotographerId = photographer.Id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CreatedDateUtc = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDateUtc { get; set; }

        public User Photographer { get; set; }
    }
}

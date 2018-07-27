using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PhotosOfUs.Model.Repositories
{
    public class PrintRepository
    {
        private PhotosOfUsContext _context;

        public PrintRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public PrintPrice GetPrice(int printId, int photographerId)
        {
            PrintPrice price = _context.PrintPrice.Where(x => x.PhotoId == printId && x.PhotographerId == photographerId).FirstOrDefault();
            return price;
        }

        public List<PrintType> GetPrintTypes()
        {
            List<PrintType> types = _context.PrintType.ToList();
            return types;
        }
    }
}

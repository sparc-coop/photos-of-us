using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public class AzureTag
    {
        public string name { get; set; }
        public double confidence { get; set; }
        public string hint { get; set; }
    }

    public class Metadata
    {
        public int height { get; set; }
        public int width { get; set; }
        public string format { get; set; }
    }

    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public List<Word> words { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public List<Line> lines { get; set; }
    }

    public class RootObject
    {
        public string language { get; set; }
        public string orientation { get; set; }
        public double textAngle { get; set; }
        public List<Region> regions { get; set; }

        public List<AzureTag> tags { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
    }
}

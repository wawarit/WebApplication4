using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication5Test.Models
{
    public class FileModel
    {
        public int id { get; set; }
        //[Required]
        public string file_name { get; set; }
        //[Required]
        public string keyword { get; set; }
        public string genre { get; set; }
        //[Required]
        public DateTime releaseDate { get; set; }

        public String dayReleaseDate { get; set; }

        public String dayEditDate { get; set; }
        //[Required]
        public string uploadFile { get; set; }
        public int view_count { get; set; }

        public int download_count { get; set; }
        public DateTime editDate { get; set; }
        public int folder_id { get; set; }
        //public byte[] uploadFile { get; set; }

        //public IEnumerable<FileModel> FileList { get; set; }

        public HttpPostedFileBase fileUpload { get; set; }
        public int TotalViews { get; set; }
    }
}
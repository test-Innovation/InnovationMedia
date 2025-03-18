using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InnovationMedia.Models;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Aspose.Slides;

namespace InnovationMedia.Controllers;

public class HomeController : Controller
{
    




    private readonly IWebHostEnvironment _env;

    public HomeController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public IActionResult Index()
    {
        string mediaPath = Path.Combine(_env.WebRootPath, "Media");
        string slidesPath = Path.Combine(mediaPath, "Slides");
        var mediList = new List<Mediamodel>();

        if (Directory.Exists(mediaPath))
        {
            var files = Directory.GetFiles(mediaPath);

            ///Converting PDF file into image
            string[] pptxFiles = Directory.GetFiles(mediaPath, "*.pptx");
            if (pptxFiles.Length > 0)
            {
                string pptxFile = pptxFiles[0];
                string pptxFilename = Path.GetFileNameWithoutExtension(pptxFile);
                string slideFolder = Path.Combine(slidesPath, pptxFilename);
                if (!Directory.Exists(slideFolder))
                {
                    Directory.CreateDirectory(slideFolder);
                    using (Presentation pres = new Presentation(pptxFile))
                    {
                        for (int i = 0; i < pres.Slides.Count; i++)
                        {
                            string imgPath = Path.Combine(slideFolder, $"slide{i + 1}.jpg");
                            pres.Slides[i].GetThumbnail(1.0f, 1.0f).Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                }
                var slideFiles = Directory.GetFiles(slideFolder, "*.jpg").OrderBy(fi => int.Parse(Path.GetFileNameWithoutExtension(fi).Replace("slide",""))).ToList();
                foreach (var f in slideFiles)
                {
                    string relPath = $"/Media/Slides/{pptxFilename}/" + Path.GetFileName(f);
                    mediList.Add(new Mediamodel { Type = "image", Source = relPath });
                }
            }
            foreach (var file in files)
            {
                string extension = Path.GetExtension(file).ToLower();
                string relativePath = "/Media/" + Path.GetFileName(file);

                string type = extension switch
                {
                    ".mp4" or ".webm" => "video",
                    ".jpg" or ".png" or ".jpeg" => "image",
                    ".pdf" => "pdf",
                    _ => "unknown"
                };
                if(type!= "unknown")
                {
                    mediList.Add(new Mediamodel { Type = type, Source = relativePath });
                }
            }
        }
        return View(mediList);
    }
}

